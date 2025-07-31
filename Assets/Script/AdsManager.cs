using UnityEngine;

public enum AdsType
{
    Banner,
    Interstitial,
    Rewarded
}

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;
    public float timeUntilNextAdBreak = 30f; // Thời gian giữa các quảng cáo
    public AdsType currentAdType = AdsType.Banner;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (currentAdType != AdsType.Banner)
        {
            // Nếu đang phát quảng cáo, không cần cập nhật thời gian
            return;
        }

        timeUntilNextAdBreak -= Time.deltaTime;

        if (timeUntilNextAdBreak <= 0f)
        {
            HandleInterstitialAdCountdown();
        }
        else
        {
            // Cập nhật UI hoặc các thành phần khác nếu cần
            UIManager.instance.SetTMPAdsTimer(Mathf.Ceil(timeUntilNextAdBreak).ToString());
        }
    }

    public void HandleInterstitialAdCountdown()
    {
        currentAdType = AdsType.Interstitial;

        // Implement your ad showing logic here
        UIManager.instance.OpenAdsBreak();
    }

    public void HandleRewardedAdCountdown()
    {
        currentAdType = AdsType.Rewarded;

        // Implement your ad showing logic here
        UIManager.instance.OpenAdsCountdownPopup();
    }

    // Methods để gọi từ Unity Button
    public void ShowInterstitialAdFromButton()
    {
        ShowAd(AdsType.Interstitial);
    }

    public void ShowRewardedAdFromButton()
    {
        ShowAd(AdsType.Rewarded);
    }

    public void ShowAd(AdsType adsType)
    {
        ShowAd(adsType, null);
    }

    public void ShowAd(AdsType adsType, System.Action callback = null)
    {
        currentAdType = adsType;

        if (adsType == AdsType.Rewarded)
        {
            Debug.Log("Showing Rewarded Ad");

            GameManager.instance.isGameOver = false; // Đặt lại trạng thái game over nếu có
            GameManager.instance.ResumeGame(); // Resume game

            UIManager.instance.CloseAdsCountdownPopup();
        }
        else if (adsType == AdsType.Interstitial)
        {
            Debug.Log("Showing Interstitial Ad");

            UIManager.instance.CloseAdsBreak();
        }
        else
        {
            Debug.LogWarning("Không có quảng cáo nào để hiển thị");
            return;
        }


        // Giả lập phát quảng cáo với callback
        StartPlayingAd(() => {
            callback?.Invoke();
            OnAdFinished();
        });
    }

    // Hàm giả lập phát quảng cáo
    private void StartPlayingAd(System.Action onAdComplete)
    {
        Debug.Log("Starting ad playback...");
        
        // Giả lập việc quảng cáo đang phát trong 5 giây
        Invoke("CompleteAd", 5f);
        
        // Lưu callback để gọi khi ad hoàn thành
        adCompleteCallback = onAdComplete;
    }
    
    private System.Action adCompleteCallback;
    
    private void CompleteAd()
    {
        Debug.Log("Ad playback completed!");
        
        // Gọi callback khi ad hoàn thành
        adCompleteCallback?.Invoke();
        adCompleteCallback = null;
    }

    public void OnAdFinished()
    {
        Debug.Log("Ad Finished");

        // Reset current ad type to Banner after ad finished
        currentAdType = AdsType.Banner;

        // Reset thời gian đếm ngược quảng cáo
        timeUntilNextAdBreak = 30f;
    }
}
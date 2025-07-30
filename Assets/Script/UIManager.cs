using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Booster Popups")]
    public static UIManager instance; // Singleton instance
    public BoosterPopup[] boosterPopups; // Gán 4 popup ở đây theo thứ tự 0-3

    private BoosterPopup currentPopup;
    public GameObject darkBG; // Gán GameObject DarkBG
    public GameObject adsCountdownPanel; // Gán GameObject AdCountdownPanel

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy đối tượng này
        }
    }

    public void OpenRateUsURL()
    {
        Application.OpenURL("https://yourlink.com");
    }
    public void OpenTermsOfUseURL()
    {
        Application.OpenURL("https://zegostudio.com/terms.html");
    }
    public void OpenPrivacyPolicyURL()
    {
        Application.OpenURL("https://zegostudio.com/privacy-policy.html");
    }

    public void OpenBoosterPopup(int index)
    {
        // Ẩn popup cũ nếu có
        if (currentPopup != null)
            currentPopup.Hide();

        // Bảo vệ chỉ số
        if (index < 0 || index >= boosterPopups.Length) return;

        currentPopup = boosterPopups[index];
        currentPopup.Show(index);
        darkBG.SetActive(true); // Hiện nền mờ

        // ❌ CHẶN KÉO khi mở popup
        GameManager.instance.draggingCircleGO.GetComponent<MoveCircle>().isBlockByUI = true;
    }

    public void CloseCurrentPopup()
    {
        if (currentPopup != null)
        {
            currentPopup.Hide();
            currentPopup = null;
        }

        // Đóng ads countdown popup nếu đang mở
        if (adsCountdownPanel != null && adsCountdownPanel.activeSelf)
        {
            adsCountdownPanel.SetActive(false);
        }

        darkBG.SetActive(false); // Ẩn nền mờ

        // ✅ CHO KÉO LẠI khi đóng popup
        if (GameManager.instance?.draggingCircleGO != null)
        {
            var moveComponent = GameManager.instance.draggingCircleGO.GetComponent<MoveCircle>();
            if (moveComponent != null)
                moveComponent.isBlockByUI = false;
        }
    }
    public void OpenAdsCountdownPopup()
    {
        if (adsCountdownPanel == null)
        {
            Debug.LogError("UIManager: adsCountdownPanel is not assigned!");
            return;
        }

        adsCountdownPanel.GetComponent<SettingPanelUI>().Show();
        darkBG.SetActive(true); // Hiện nền mờ

        // ❌ CHẶN KÉO khi mở popup
        if (GameManager.instance?.draggingCircleGO != null)
        {
            var moveComponent = GameManager.instance.draggingCircleGO.GetComponent<MoveCircle>();
            if (moveComponent != null)
                moveComponent.isBlockByUI = true;
        }
    }
    
    public void CloseAdsCountdownPopup()
    {
        if (adsCountdownPanel != null)
        {
            adsCountdownPanel.SetActive(false);
            darkBG.SetActive(false); // Ẩn nền mờ
        }
    }
}

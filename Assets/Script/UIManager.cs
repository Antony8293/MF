using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Booster Popups")]
    public static UIManager instance; // Singleton instance
    public BoosterPopup[] boosterPopups; // Gán 4 popup ở đây theo thứ tự 0-3

    private BoosterPopup currentPopup;
    public GameObject darkBG; // Gán GameObject DarkBG
    public GameObject adsCountdownPanel; // Gán GameObject AdCountdownPanel

    public float adsCountdownTime = 30f; // Thời gian đếm ngược quảng cáo
    public TextMeshProUGUI adsCountdownText; // Gán TMP_Text để hiển thị thời gian đếm ngược

    public GameObject adBreakPanel; // Gán GameObject AdBreakPanel
    public SettingPanelUI pausePanel; // Gán GameObject PausePanel
    public GameObject pipe; // Gán GameObject Pipe
    public GameObject canvas_world; // Gán GameObject CanvasWorld
    public GameObject canvas_camera; // Gán GameObject CanvasCamera 
    public Camera camera; // Gán Camera để sử dụng trong UIScaleShakingBoosterEffect
    private Vector3 originalCameraPosition; // Lưu vị trí gốc của camera

    public GameObject boxSprite;
    public GameObject boxCollider; // Gán GameObject BoxCollider để sử dụng trong UIScaleShakingBoosterEffect
    private Vector3 originalBoxSpritePosition; // Lưu vị trí gốc của BoxSprite
    private Vector3 originalBoxColliderPosition; // Lưu vị trí gốc của BoxCollider
    public float scaleShakeDuration = 0.5f; // Thời gian hiệu ứng scale khi sử dụng booster Shake the Box

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

    private void Start()
    {
        originalCameraPosition = camera.transform.position; // Lưu vị trí gốc của camera
        originalBoxSpritePosition = boxSprite.transform.position; // Lưu vị trí gốc của BoxSprite
        originalBoxColliderPosition = boxCollider.transform.position; // Lưu vị trí gốc của BoxCollider
    }

    private void Update()
    {
        // if (adsCountdownTime > 0)
        // {
        //     adsCountdownTime -= Time.deltaTime; // Giảm thời gian đếm ngược
        //     adsCountdownText.text = Mathf.Ceil(adsCountdownTime).ToString(); // Cập nhật văn bản đếm ngược
        // }
        // else
        // {
        //     OpenAdsBreak(); // Mở popup quảng cáo khi hết thời gian
        // }

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

    public void OpenAdsBreak()
    {
        if (adBreakPanel == null)
        {
            Debug.LogError("UIManager: adBreakPanel is not assigned!");
            return;
        }

        GameManager.instance.SetBlockFruitDragging(true); // Chặn kéo khi mở quảng cáo

        adBreakPanel.GetComponent<SettingPanelUI>().Show();
        darkBG.SetActive(true); // Hiện nền mờ
    }

    public void SetTMPAdsTimer(string text)
    {
        if (adsCountdownText == null)
        {
            Debug.LogError("UIManager: adsCountdownText is not assigned!");
            return;
        }

        adsCountdownText.text = text; // Cập nhật văn bản đếm ngược
    }

    public void CloseAdsBreak()
    {
        adsCountdownTime = 30f; // Dừng đếm ngược quảng cáo
        GameManager.instance.SetBlockFruitDragging(false);

        if (adBreakPanel != null)
        {
            adBreakPanel.SetActive(false);
            darkBG.SetActive(false); // Ẩn nền mờ
        }
    }

    public void OpenPausePanel()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("pausePanel is null!");
            return;
        }

        pausePanel.Show(); // DOTween show
        darkBG.SetActive(true); // Hiện nền mờ


        GameManager.instance.PauseGame();
    }

    public void ClosePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.Hide(); // DOTween hide
            darkBG.SetActive(false); // Ẩn nền mờ
        }

        GameManager.instance.ResumeGame(); // Đóng popup và tiếp tục game
    }

    public void UIScaleShakingBoosterEffect(bool isStartEffect)
    {
        if (isStartEffect == Const.START_EFFECT)
        {
            GameManager.instance.draggingCircleGO.SetActive(false); // Ẩn đối tượng kéo khi bắt đầu hiệu ứng
            GameManager.instance.nextCircleGO.SetActive(false); // Ẩn đối tượng tiếp theo khi bắt đầu hiệu ứng

            camera.orthographic = false; // Đảm bảo camera là perspective
            pipe.SetActive(false);
            canvas_world.SetActive(true);
            canvas_camera.SetActive(false);

            boxCollider.transform.SetParent(boxSprite.transform); // Đặt BoxCollider làm con của BoxSprite


            Sequence sequence = DOTween.Sequence();
            sequence.Append(camera.transform.DOMove(new Vector3(0, 0, -19f), scaleShakeDuration).SetEase(Ease.OutQuint))
                    .Join(boxSprite.transform.DOMove(originalBoxSpritePosition + new Vector3(0, 0.3f, 0), scaleShakeDuration + 0.2f).SetEase(Ease.OutBack))
                    .OnComplete(() =>
                    {
                        Booster.Booster4Clicked();
                    });
        }
        else
        {
            // Trở về trạng thái ban đầu
            Sequence sequence = DOTween.Sequence();

            sequence.Append(boxSprite.transform.DOMove(originalBoxSpritePosition, scaleShakeDuration + 0.15f).SetEase(Ease.OutBounce))
                    .Join(camera.transform.DOMove(
                        originalCameraPosition, // Vị trí cũ của camera
                        scaleShakeDuration // Thời gian hiệu ứng
                    ).SetEase(Ease.OutQuint))
                    .OnComplete(() =>
                    {
                        camera.orthographic = true; // Đổi về orthographic
                        pipe.SetActive(true);
                        canvas_world.SetActive(false);
                        canvas_camera.SetActive(true);

                        GameManager.instance.draggingCircleGO.SetActive(true); // Hiện đối tượng kéo
                        GameManager.instance.nextCircleGO.SetActive(true); // Hiện đối tượng tiếp theo

                        boxCollider.transform.SetParent(null); // Tách BoxCollider ra khỏi BoxSprite

                        GameManager.instance.isBoosterTriggered = false; // Đánh dấu đã kết thúc hiệu ứng
                    });
        }

    }
}

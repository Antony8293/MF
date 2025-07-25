using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoosterPopup : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform popupBox;
    public Text boosterTitle;

    private Vector3 originalScale;  // scale gốc của popup
    private UIManager uiManager;
    private void Awake()
    {
        originalScale = popupBox.localScale;  // Lưu scale gốc một lần
        uiManager = FindObjectOfType<UIManager>(); // Tìm UIManager trong scene
    }

    public void Show(int boosterIndex)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        popupBox.localScale = Vector3.zero;

        canvasGroup.DOFade(1f, 0.25f);
        popupBox.DOScale(originalScale.x, 0.3f).SetEase(Ease.OutBack);

        string[] names = { "Boom", "Hammer", "Upgrade", "Shuffle" };
        if (boosterTitle != null)
            boosterTitle.text = names[boosterIndex];
    }

    public void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f);
        popupBox.DOScale(0f, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void OnCloseClicked()
    {
        uiManager?.CloseCurrentPopup(); // Gọi về UIManager
    }

    public void OnUseBoosterClicked(int boosterIndex)
    {
        if (boosterIndex < 0 || boosterIndex >= 4) return;

        // Gọi hàm sử dụng booster tương ứng
        switch (boosterIndex)
        {
            case 0:
                // Sử dụng booster Smallest
                uiManager?.CloseCurrentPopup(); // Đóng popup sau
                Booster.Booster1Clicked();
                break;
            case 1:
                // Sử dụng booster Hammer
                uiManager?.CloseCurrentPopup(); // Đóng popup sau
                Booster.Booster2Clicked();
                break;
            case 2:
                // Sử dụng booster Upgrade
                uiManager?.CloseCurrentPopup(); // Đóng popup sau
                Booster.Booster3Clicked();
                break;
            case 3:
                // Sử dụng booster Shake the Box
                uiManager?.CloseCurrentPopup(); // Đóng popup sau
                Booster.Booster4Clicked();
                break;
        }
    }
}
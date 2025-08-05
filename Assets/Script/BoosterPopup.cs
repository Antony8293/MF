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
        popupBox.DOScale(0f, 0.2f).SetEase(Ease.InBack);
    }

    public void OnCloseClicked()
    {
        uiManager?.CloseCurrentPopup(); // Gọi về UIManager
    }

    public void OnUseBoosterClicked(int boosterIndex)
    {
        if (boosterIndex < BoosterManager.BOOSTER_NON || boosterIndex >= BoosterManager.BOOSTER_SHAKE) return;

        AdsManager.instance.ShowAd(AdsType.Rewarded, () =>
        {
            uiManager?.CloseCurrentPopup(); // Đóng popup sau
            BoosterManager.instance.UseBooster(boosterIndex, gameObject);
        });


    }

   
}
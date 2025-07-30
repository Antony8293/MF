using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;

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
            // .OnComplete(() => gameObject.SetActive(false));
    }

    public void OnCloseClicked()
    {
        uiManager?.CloseCurrentPopup(); // Gọi về UIManager
    }

    public void OnUseBoosterClicked(int boosterIndex)
    {
        if (boosterIndex < 0 || boosterIndex >= 4) return;
        uiManager?.CloseCurrentPopup();
        StartCoroutine(DelayBoosterClicked(boosterIndex));
    }

    private IEnumerator DelayBoosterClicked(int boosterIndex)
    {
        yield return new WaitForSeconds(0.3f);
        switch (boosterIndex)
        {
            case 0:
                Booster.Booster1Clicked();
                break;
            case 1:
                Booster.Booster2Clicked();
                break;
            case 2:
                Booster.Booster3Clicked();
                break;
            case 3:
                // Sử dụng booster Shake the Box
                uiManager.UIScaleShakingBoosterEffect(Const.START_EFFECT); // Hiệu ứng scale
                break;
        }
        gameObject.SetActive(false); // Ẩn popup sau khi sử dụng booster
    }
}
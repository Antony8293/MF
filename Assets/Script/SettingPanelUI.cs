using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SettingPanelUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;     // Gán component CanvasGroup
    public Transform panelContainer;    // Gán panel cần scale (ví dụ chính nó)

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = Vector3.one; // ép về (1,1,1) chuẩn
    }

    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        panelContainer.localScale = Vector3.zero;

        canvasGroup.DOFade(1f, 0.25f).SetUpdate(true);
        panelContainer.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f).SetUpdate(true);
        panelContainer.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }


    public void OnClickClose()
    {
        Hide();
    }
}

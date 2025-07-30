using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class AdsCountdownPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        canvasGroup.DOFade(1f, 0.25f);
        panelContainer.DOScale(originalScale.x, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f);
        panelContainer.DOScale(0f, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}

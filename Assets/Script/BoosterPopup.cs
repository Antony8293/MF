using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoosterPopup : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform popupBox;
    public Text boosterTitle;

    private Vector3 originalScale;  // lệ gốc của popup
    private void Awake()
    {
        originalScale = popupBox.localScale;  // Lưu scale gốc một lần
    }

    public void Show(int boosterIndex)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        popupBox.localScale = Vector3.zero;

        canvasGroup.DOFade(1f,0.25f);
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
        FindObjectOfType<UIManager>()?.CloseCurrentPopup(); // Gọi về UIManager
    }
}

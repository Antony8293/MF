using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoosterPopup : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Transform popupBox;
    public Text boosterTitle;

    private float originalScale;  // lệ gốc của popup

    public void Show(int boosterIndex)
    {
        originalScale = transform.localScale.x; // Tỷ lệ gốc của popup

        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        popupBox.localScale = Vector3.zero;

        canvasGroup.DOFade(1f,0.25f);
        popupBox.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack);

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

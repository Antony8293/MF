using UnityEngine;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public CanvasGroup tutorialPanel;
    public GameObject handPointer;

    private const string TutorialKey = "HasSeenTutorial";

    private Tween handTween;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Start()
    {
#if UNITY_EDITOR
        // Reset nếu cần test
        PlayerPrefs.SetInt(TutorialKey, 0);
#endif

        if (!HasSeenTutorial())
        {
            ShowTutorial();
        }
    }

    private bool HasSeenTutorial() => PlayerPrefs.GetInt(TutorialKey, 0) == 1;

    private void MarkAsSeen()
    {
        GameManager.instance.isPlayingTutorial = false;
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();
    }

    public void ShowTutorial()
    {
        GameManager.instance.isPlayingTutorial = true;

        tutorialPanel.gameObject.SetActive(true);
        tutorialPanel.alpha = 1;
        tutorialPanel.interactable = true;
        tutorialPanel.blocksRaycasts = true;

        handTween?.Kill();
        handTween = handPointer.transform
            .DOScale(1.2f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(gameObject);
    }

    public void CloseTutorial()
    {
        handTween?.Kill();

        tutorialPanel.alpha = 0;
        tutorialPanel.interactable = false;
        tutorialPanel.blocksRaycasts = false;
        tutorialPanel.gameObject.SetActive(false);

        MarkAsSeen();
    }
}

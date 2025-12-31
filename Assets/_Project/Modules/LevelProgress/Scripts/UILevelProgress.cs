using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UILevelProgress : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] Image progressBar;
    [SerializeField] Image backgroundBar;

    //Pop up
    [SerializeField] UIYouReached youReachedUI;

    private void OnEnable()
    {
        LevelManager.OnProgressChange += UpdateProgressUI;
        LevelManager.OnNextLevel += OnNextLevelUI;
    }

    private void OnDisable()
    {
        LevelManager.OnProgressChange -= UpdateProgressUI;
        LevelManager.OnNextLevel -= OnNextLevelUI;
    }

    private void OnNextLevelUI(int level)
    {
        ResetBar();
        ShowYouReachedUI(level);
    }

    private void ShowYouReachedUI(int level)
    {
        youReachedUI.gameObject.SetActive(true);
        youReachedUI.SetLevelReached(level);
    }

    private void ResetBar()
    {
        progressBar.fillAmount = 0f;
    }

    private void UpdateProgressUI(int score, int level, float progress)
    {
        this.level.text = level.ToString();
        backgroundBar.fillAmount = progress;

        //Hiệu ứng cho text điểm và progress bar
        SetUIDelay(score).Forget();
    }

    async UniTaskVoid SetUIDelay(int endScore)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        int startScore;
        int.TryParse(score.text, out startScore);

        float startFill = progressBar.fillAmount;
        float endFill = backgroundBar.fillAmount;

        float duration = 1f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalizeTime = t / duration; // 0 -> 1

            int displayScore = Mathf.RoundToInt(Mathf.Lerp(startScore, endScore, normalizeTime));
            score.text = displayScore.ToString();

            progressBar.fillAmount = Mathf.Lerp(startFill, endFill, normalizeTime);
            await UniTask.Yield();
        }

        score.GetComponent<Animator>().SetTrigger("ScaleUp");
    }
}

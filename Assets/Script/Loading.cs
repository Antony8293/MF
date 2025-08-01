using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Loading : MonoBehaviour
{
    public static string NEXT_SCENE = "GamePlay"; // Tên scene cần load, đổi nếu khác
    public GameObject progressBar;
    public TextMeshProUGUI loadingText;
    private float loadingTime = 10f; // Thời gian cố định để hiển thị loading
    void Start()
    {
        StartCoroutine(LoadScene(NEXT_SCENE));
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(NEXT_SCENE); // đổi tên nếu khác
        operation.allowSceneActivation = false;

        // while (operation.progress < 0.9f)
        // {
        //     // float progress = Mathf.Clamp01(operation.progress / 0.9f); // Chia cho 0.9 để tiến độ đạt tối đa là 1
        //     // progressBar.GetComponent<Image>().fillAmount = progress;
        //     // loadingText.text = $"Loading... {progress * 100f:0}%";

        //     float progress = Mathf.Lerp(progressBar.GetComponent<Image>().fillAmount, operation.progress, Time.deltaTime * fixedLoadingTime);
        //     progressBar.GetComponent<Image>().fillAmount = progress;
        //     loadingText.SetText($"Loading...");

        //     yield return null;
        // }

        // // Đợi thêm để đầy 100% và kéo dài thời gian loading
        // float t = 0;
        // float extraLoadingTime = 3f; // Thời gian loading thêm (giây)
        // while (t < extraLoadingTime)
        // {
        //     t += Time.deltaTime;
        //     float percent = Mathf.Clamp01(t / extraLoadingTime);
        //     progressBar.GetComponent<Image>().fillAmount = Mathf.Lerp(progressBar.GetComponent<Image>().fillAmount, 1f, percent);
        //     yield return null;
        // }

        // yield return new WaitForSeconds(1.5f); // Thêm thời gian chờ cuối
        // operation.allowSceneActivation = true;
        


        // Loading đều trong 90% thời gian đầu tiên
        float timer = 0f;
        while (timer < loadingTime)
        {
            timer += Time.deltaTime;
            float percent = Mathf.Clamp01(timer / loadingTime);
            progressBar.GetComponent<Image>().fillAmount = percent;
            loadingText.SetText($"Loading... {percent * 100f:0}%");
            yield return null;
        }
        yield return new WaitForSeconds(1f); // Thêm thời gian chờ cuối
        operation.allowSceneActivation = true;
    }
}

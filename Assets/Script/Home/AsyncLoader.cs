using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    public static AsyncLoader _instance;

    [SerializeField]
    private GameObject loadingScreen;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private Slider loadingSlider;

    void Awake()
    {
        if (_instance == null && _instance != this) _instance = this;
        else Destroy(this);
    }

    public void LoadLevelBtn(string levelToLoad)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }
}

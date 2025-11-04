using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{

    public static HomeManager _instance;

    [SerializeField]
    private GameObject HomeCanvas;

    [SerializeField]
    private GameObject AdvCanvas;

    void Awake()
    {
        if (_instance == null && _instance != this) _instance = this;
        else Destroy(this);
    }

    void OnEnable()
    {
        ModeManager.advChosen += turnOnAdvMode;
        ModeManager.classicChosen += turnOnClassicMode;
        ScrollLevelLogger.onLevelChoosen += onLevelChoosen;
    }

    void OnDisable()
    {
        ModeManager.advChosen -= turnOnAdvMode;
        ModeManager.classicChosen -= turnOnClassicMode;
        ScrollLevelLogger.onLevelChoosen -= onLevelChoosen;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void turnOnAdvMode()
    {
        HomeCanvas.SetActive(false);
        AdvCanvas.SetActive(true);
    }

    private void turnOnClassicMode()
    {

    }
    
    private void onLevelChoosen(int level)
    {
        if (level == 1)
            AsyncLoader._instance.LoadLevelBtn("Level1");
    }

}

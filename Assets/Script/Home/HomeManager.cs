using UnityEngine;

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

}

using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager _instance;

    void Awake()
    {
        if( _instance == null && _instance != this) _instance = this;
        else Destroy(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

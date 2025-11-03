using System;
using UnityEngine;

public class ModeManager : MonoBehaviour
{

    public static ModeManager _instance;

    public static Action advChosen;

    public static Action classicChosen;

    void Awake()
    {
        if( _instance == null && _instance != this) _instance = this;
        else Destroy(this);
    }

    public void ChooseAdvMode() => advChosen?.Invoke();

    public void ChooseClassicMode() => classicChosen?.Invoke();
}

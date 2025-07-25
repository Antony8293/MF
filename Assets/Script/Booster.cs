using System;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public static event Action boosTer1;

    public static event Action booster2;

    public static event Action booster3;

    public static event Action booster4;

    public static void Booster1Clicked() => boosTer1?.Invoke();

    public static void Booster2Clicked() => booster2?.Invoke();

    public static void Booster3Clicked() => booster3?.Invoke();

    public static void Booster4Clicked() => booster4?.Invoke();
}

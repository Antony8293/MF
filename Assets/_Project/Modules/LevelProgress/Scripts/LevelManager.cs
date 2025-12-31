using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public static class LevelProgress
{
    public static readonly string LEVEL_SCORE = "LevelScore";
    public static readonly string LEVEL_CURRENT = "LevelCurrent";
    public static readonly string LEVEL_PROGRESS = "LevelProgress";

}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static event Action<int, int, float> OnProgressChange;
    public static event Action<int> OnNextLevel;

    static private int scoreMilestone = 300;
    private int currentScore;
    private int currentLevel;
    private float currentProgress;

    void OnEnable()
    {
        //GameManager.OnScoreChanged += SetScore;
    }

    void OnDisable()
    {
        //GameManager.OnScoreChanged -= SetScore;
    }

    private void Start()
    {
        LoadData();

        OnProgressChange?.Invoke(currentScore, currentLevel, currentProgress);
    }

    public void SetScore(int addScore)
    {
        currentScore += addScore;
        currentProgress += (float) addScore / scoreMilestone;

        // Kiểm tra nếu qua level mới
        if (currentProgress > 1)
        {
            currentProgress -= 1;
            currentLevel++;

            OnNextLevel?.Invoke(currentLevel);
        }

        OnProgressChange?.Invoke(currentScore, currentLevel, currentProgress);
        SaveData();
    }

    

    //Cập nhật PlayerPref
    private void SaveData()
    {
        PlayerPrefs.SetInt(LevelProgress.LEVEL_SCORE, currentScore);
        PlayerPrefs.SetInt(LevelProgress.LEVEL_CURRENT, currentLevel);
        PlayerPrefs.SetFloat(LevelProgress.LEVEL_PROGRESS, currentProgress);
    }

    //Gán dữ liệu PlayerPref
    private void LoadData()
    {
        currentScore = PlayerPrefs.GetInt(LevelProgress.LEVEL_SCORE, 0);
        currentLevel = PlayerPrefs.GetInt(LevelProgress.LEVEL_CURRENT, 1);
        currentProgress = PlayerPrefs.GetFloat(LevelProgress.LEVEL_PROGRESS, 0f);
    }

}

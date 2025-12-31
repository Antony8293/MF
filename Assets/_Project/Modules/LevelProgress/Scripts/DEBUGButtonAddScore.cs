using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGButtonAddScore : MonoBehaviour
{
    private int totalScore;

    [SerializeField] int minScore;
    [SerializeField] int maxScore;

    public static DEBUGButtonAddScore instance;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        totalScore = PlayerPrefs.GetInt("Score", 0);
    }
    public void OnClickSendScore()
    {
        int addScore = Random.Range(minScore, maxScore + 1);
        totalScore += addScore;

        LevelManager.Instance.SetScore(addScore);

        Debug.Log("Cộng điểm:" + addScore + "; Tổng điểm:" + totalScore);
        
    }
}

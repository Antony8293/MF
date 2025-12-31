//using DailyTask;
using DailyReward;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIYouReached : UICanvas
{
    // Start is called before the first frame update
    [SerializeField] UIClick _collectCoinBtn;
    [SerializeField] UIClick _collectAdCoinBtn;

    [SerializeField] TextMeshProUGUI levelReached;

    protected override void OnEnable()
    {
        base.OnEnable();

        _collectCoinBtn.ActionAfterClick += HandleCollectCoin;
        _collectAdCoinBtn.ActionAfterClick += HandleCollectAdCoin;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _collectCoinBtn.ActionAfterClick -= HandleCollectCoin;
        _collectAdCoinBtn.ActionAfterClick -= HandleCollectAdCoin;
    }

    void HandleCollectCoin()
    {
        CloseUI();
        // AdManager.Instance.ShowRewardedAd(() =>
        Debug.Log("collect");
    }

    void HandleCollectAdCoin()
    {
        CloseUI();
        Debug.Log("collect ad");
    }

    public void SetLevelReached(int level)
    {
        levelReached.text = "Level " + level.ToString();
    }
}

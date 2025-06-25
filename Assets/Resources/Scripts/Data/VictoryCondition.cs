using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameEndConditionList
{
    ScoreGoal,
    LiveAlone,
    TimeOut,
}

public enum BonusConditionList
{
     Delivery1st,
     Hit1st,
     MinimumGetItem,
}

public class VictoryCondition : MonoBehaviour
{
    List<Sprite> bonusVictoryImage;


    private void Start()
    {
        Manager.Instance.observer.OnGameEnd += CheckWinCondition;
    }



    public void CheckWinCondition()
    {
        if(Manager.Instance.observer.isGameOver == true)
        {
           
        }

    }

    public void BonusWinCondition()
    {

    }
}

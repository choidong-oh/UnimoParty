using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        Manager.Instance.observer.OnGameEnd += CheckWinCondition;
    }



    public void CheckWinCondition()
    {
        if(Manager.Instance.observer.isGameOver == true)
        {
            switch(Manager.Instance.players.Count <= 1)
            {

            }
        }

    }
}

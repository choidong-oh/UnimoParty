using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameObserver
{
    public event Action<int> OnGameDataChange;
    public event Action OnGameEnd;

    public UserPlayer UserPlayer { get; private set; }
    private int gameoverTargetScore = 100;
    private DataCenter gamedata = new DataCenter(3, 0);
    private bool isGameOver = false;


    public void HitPlayer(int damage)
    {
        gamedata.life = gamedata.life - damage;
        var templife = gamedata.life;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(templife);

        if (gamedata.life <= 0)
        {
            isGameOver = true;
            OnGameEnd.Invoke();
        }
    }

    public void AddScore(int score)
    {
        gamedata.score = gamedata.score + score;
        var tempscore = gamedata.score;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempscore);

        if(gamedata.score >= gameoverTargetScore)
        {
            isGameOver = true;
            OnGameEnd.Invoke();
        }
    }

    public void ResetPlayer()
    {

    }

    public void EndGame()
    {
        if(isGameOver == true)
        {
            OnGameEnd.Invoke();
        }
    }
}

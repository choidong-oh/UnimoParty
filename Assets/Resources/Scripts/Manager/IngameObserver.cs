using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameObserver
{
    public event Action<DataCenter> OnGameDataChange;
    public event Action OnGameEnd;

    public UserPlayer UserPlayer { get; private set; }
    private int gameoverTargetScore = 100;
    private DataCenter gamedata = new DataCenter(3, 0);
    private bool isGameOver = false;


    public void HitPlayer(int damage)
    {
        gamedata.life = gamedata.life - damage;
        var templife = gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(templife);

        if (gamedata.life <= 0)
        {
            isGameOver = true;
            OnGameEnd.Invoke();
        }
    }

    public void GetFairy(PlayerFairy fairytype)
    {
        //페어리 타입을 이미 전부 받은 상태에서 유저 페어리에 대입함.
        gamedata.playerFairyType = fairytype;
        var tempfairy = gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempfairy);
    }


    public void AddScore(int score)
    {
        gamedata.score = gamedata.score + score;
        var tempscore = gamedata;

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
            //여기에 포톤 추가.
            OnGameEnd.Invoke();
        }
    }
}

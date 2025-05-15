using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameObserver
{
    public event Action<DataCenter> OnGameDataChange;
    public event Action OnGameEnd;

    public UserPlayer UserPlayer { get; set; }
    private FairyType tempPlayerFairy;
    private int gameoverTargetScore = 100;

    private bool isGameOver = false;


    public void HitPlayer(int damage)
    {
        UserPlayer.gamedata.life = UserPlayer.gamedata.life - damage;
        var templife = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(templife);

        if (UserPlayer.gamedata.life <= 0)
        {
            isGameOver = true;

            // 여기에 포톤 추가.
            OnGameEnd.Invoke();
        }
    }

    public void GetFairy(FairyType fairytype)
    {
        //페어리 타입을 이미 전부 받은 상태에서 유저 페어리에 대입함.
        UserPlayer.gamedata.playerFairyType = fairytype;
        var tempfairy = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempfairy);
    }

    public void RetrunFairy()
    {
        tempPlayerFairy.FairyDataType_1 = 0;
        tempPlayerFairy.FairyDataType_2 = 0;
        tempPlayerFairy.FairyDataType_3 = 0;

        UserPlayer.gamedata.playerFairyType = tempPlayerFairy;
        var tempfairy = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempfairy);
    }

    public void AddScore(int score)
    {
        UserPlayer.gamedata.score = UserPlayer.gamedata.score + score;
        var tempscore = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempscore);

        if(UserPlayer.gamedata.score >= gameoverTargetScore)
        {
            isGameOver = true;

            // 여기에 포톤 추가.
            OnGameEnd.Invoke();
        }
    }

    public void ResetPlayer()
    {
        var tempPlayer = UserPlayer.gamedata.Clone();

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempPlayer);
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

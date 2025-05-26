using System;
using UnityEngine.SceneManagement;

public class IngameObserver
{
    public event Action<DataCenter> OnGameDataChange;
    public event Action OnGameEnd;

    public UserPlayer UserPlayer { get; set; }
    private FairyType tempPlayerFairy;
    private int gameoverTargetScore = 100;

    private bool isGameOver = false;

    public void Setting()
    {
        UserPlayer.gamedata.life = 100;
        var tempUser = UserPlayer.gamedata;

        OnGameDataChange.Invoke(tempUser);
    }


    public void HitPlayer(int damage)
    {
        UserPlayer.gamedata.life = UserPlayer.gamedata.life - damage;
        var templife = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(templife);

        if (UserPlayer.gamedata.life <= 0)
        {

            UserPlayer.gamedata.life = 20;
            isGameOver = true;

            SceneManager.LoadScene(1);

            // 여기에 포톤 추가.
            // OnGameEnd?.Invoke();
        }
    }

    public void GetFairy(FairyType fairytype)
    {
        //페어리 타입을 이미 받은 상태에서 유저 페어리에 대입함.
        UserPlayer.gamedata.playerFairyType = fairytype;
        var tempfairy = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempfairy);
    }

    public void DeliveryFairy()
    {
        Manager.Instance.goalCount.GoalFairyValue_1 += UserPlayer.gamedata.playerFairyType.FairyDataType_1;
        Manager.Instance.goalCount.GoalFairyValue_2 += UserPlayer.gamedata.playerFairyType.FairyDataType_2;
        Manager.Instance.goalCount.GoalFairyValue_3 += UserPlayer.gamedata.playerFairyType.FairyDataType_3;

        tempPlayerFairy.FairyDataType_1 = 0;
        tempPlayerFairy.FairyDataType_2 = 0;
        tempPlayerFairy.FairyDataType_3 = 0;

        UserPlayer.gamedata.playerFairyType = tempPlayerFairy;
        var tempfairy = UserPlayer.gamedata;

        if(Manager.Instance.goalCount.GoalFairyValue_1 == Manager.Instance.tempFairyValue_1 && Manager.Instance.goalCount.GoalFairyValue_2 == Manager.Instance.tempFairyValue_2 && Manager.Instance.goalCount.GoalFairyValue_3 == Manager.Instance.tempFairyValue_3)
        {
            isGameOver = true;
            OnGameEnd?.Invoke();
        }
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
            OnGameEnd?.Invoke();
        }
    }

    public void ResetPlayer()
    {
        var tempPlayer = UserPlayer.gamedata.Clone();

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempPlayer);
    }

    //강제로 게임을 종료시키는 메소드.
    public void EndGame()
    {
        if(isGameOver == true)
        {
            //여기에 포톤 추가.
            OnGameEnd?.Invoke();
        }
    }
}

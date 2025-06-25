using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameObserver
{
    public event Action<DataCenter> OnGameDataChange;
    public event Action<List<int>> OnGameRankChange;
    public event Action OnGameEnd;

    public UserPlayer UserPlayer { get; set; }
    private FairyType tempPlayerFairy;
    private int gameoverTargetScore = 100;
    public int roomInPlayerCount;
    public List<int> ranks;
    public int currentPlayerRank;

    public bool isGameOver = false;
    public int mainGameOverNum;
    public int subGameOverNum;
    ItemData _selectItem;
    public ItemGetRate getItemRate = new ItemGetRate();
    System.Random _itemRandomNum;

    public void Setting()
    {
        UserPlayer.gamedata.life = 100;
        var tempUser = UserPlayer.gamedata;

        OnGameDataChange.Invoke(tempUser);
    }

    public void HitPlayer(int damage)
    {
        if (UserPlayer.gamedata._playerState == PlayerState.None)
        {
            UserPlayer.gamedata.life = UserPlayer.gamedata.life - damage;
        }
        else if (UserPlayer.gamedata._playerState == PlayerState.Invincible)
        {
            UserPlayer.gamedata.life = UserPlayer.gamedata.life = 0;
        }

        var templife = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(templife);

        Manager.Instance.observer.UserPlayer.gamedata.hitcount++;

        if (UserPlayer.gamedata.life <= 0)
        {
            isGameOver = true;

            SceneManager.LoadScene(1);

            //if(LobbyManager.엑터 넘버 리스트. 플레이어 카운트가 <= 1)
            //{
            //    Manager.Instance.observer.mainGameOverNum = (int)GameEndConditionList.LiveAlone;
            //    OnGameEnd?.Invoke();
            //}
        }
    }

    public void RecoveryPlayerHP(int RecoveryHP)
    {
        UserPlayer.gamedata.life += RecoveryHP;

        if (UserPlayer.gamedata.life >= 100)
        {
            UserPlayer.gamedata.life = 100;
        }

        var templife = UserPlayer.gamedata;
        OnGameDataChange?.Invoke(templife);
    }

    void GetItem()
    {
        var randomrange = _itemRandomNum.Next((int)ItemName.Potion, (int)ItemName.end);
        switch (randomrange)
        {
            //아이템 enum 크기만큼 case 생성하여 userItemDatas의 같은 값을 대입
            case (int)ItemName.Potion:
                Manager.Instance.observer.UserPlayer.gamedata._Inventory.userItemDatas[(int)ItemName.Potion].ItemData.ItemCount++;
                break;

            case (int)ItemName.FreezeBoom:
                Manager.Instance.observer.UserPlayer.gamedata._Inventory.userItemDatas[(int)ItemName.FreezeBoom].ItemData.ItemCount++;
                break;

            case (int)ItemName.Barricade:
                Manager.Instance.observer.UserPlayer.gamedata._Inventory.userItemDatas[(int)ItemName.FreezeBoom].ItemData.ItemCount++;
                break;

            case (int)ItemName.end:
                break;
        }

        var tempUser = UserPlayer.gamedata;

        OnGameDataChange.Invoke(tempUser);
    }

    void UseItem(ItemData selectitem)
    {
        switch(selectitem.type)
        {
            case ItemName.Potion:
                //손에 아이템을 들어 사용 했다는 기능을 추가
                Manager.Instance.observer.UserPlayer.gamedata._Inventory.userItemDatas[(int)ItemName.Potion].ItemData.ItemCount--;
                break;

            case ItemName.FreezeBoom:
                Manager.Instance.observer.UserPlayer.gamedata._Inventory.userItemDatas[(int)ItemName.FreezeBoom].ItemData.ItemCount--;
                break;

            case ItemName.end:
                break;
        }

        var tempUser = Manager.Instance.observer.UserPlayer.gamedata;
        OnGameDataChange.Invoke(tempUser);
    }

    void SelectItem(ItemData selectitem)
    {
        _selectItem = selectitem;

        var tempuser = Manager.Instance.observer.UserPlayer.gamedata;

        OnGameDataChange.Invoke(tempuser);
    }

    public void GetFairy(FairyType fairytype)
    {
        //페어리 타입을 이미 받은 상태에서 유저 페어리에 대입함.
        UserPlayer.gamedata.playerFairyType.FairyDataType_1 += fairytype.FairyDataType_1;
        UserPlayer.gamedata.playerFairyType.FairyDataType_2 += fairytype.FairyDataType_2;
        UserPlayer.gamedata.playerFairyType.FairyDataType_3 += fairytype.FairyDataType_3;

        var tempfairy = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempfairy);
    }

    public void DeliveryFairy()
    {
        Manager.Instance.tempFairyValue_1 -= UserPlayer.gamedata.playerFairyType.FairyDataType_1;
        Manager.Instance.tempFairyValue_2 -= UserPlayer.gamedata.playerFairyType.FairyDataType_2;
        Manager.Instance.tempFairyValue_3 -= UserPlayer.gamedata.playerFairyType.FairyDataType_3;

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

        Manager.Instance.observer.UserPlayer.gamedata.deliveryCount++;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempfairy);

        Debug.Log("반납 후 점수" + Manager.Instance.observer.UserPlayer.gamedata.score);
    }

    public void AddScore()
    {
        var tempscore1 = UserPlayer.gamedata.playerFairyType.FairyDataType_1 * Manager.Instance._FairyScore_1;
        var tempscore2 = UserPlayer.gamedata.playerFairyType.FairyDataType_2 * Manager.Instance._FairyScore_2;
        var tempscore3 = UserPlayer.gamedata.playerFairyType.FairyDataType_3 * Manager.Instance._FairyScore_3;
        var tempscore = tempscore1 + tempscore2 + tempscore3;

        UserPlayer.gamedata.score = UserPlayer.gamedata.score + tempscore;
        var tempuser = UserPlayer.gamedata;

        //여기에 포톤 추가.
        OnGameDataChange?.Invoke(tempuser);

        if(UserPlayer.gamedata.score >= gameoverTargetScore)
        {
            isGameOver = true;

            // 여기에 포톤 추가.
            OnGameEnd?.Invoke();
        }
    }

    public void BuyItem(ItemData buyitems)
    {
        UserPlayer.gamedata._money -= buyitems.ItemCost;
        UserPlayer.gamedata._Inventory.userItemDatas[(int)buyitems.type].ItemData.ItemCount++;

        var tempuser = UserPlayer.gamedata;

        OnGameDataChange?.Invoke(tempuser);
    }

    public void BuyShip(SpaceShipData buyship)
    {
        
        UserPlayer.gamedata._money -= buyship.ShipCost;
        UserPlayer.gamedata._Inventory.spaceShipDatas.Add(buyship);

        var tempuser = UserPlayer.gamedata;

        OnGameDataChange?.Invoke(tempuser);
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
        isGameOver = true;
         
        //여기에 포톤 추가.
        OnGameEnd?.Invoke();
    }
}

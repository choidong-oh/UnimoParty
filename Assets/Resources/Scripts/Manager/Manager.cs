using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager instance;
    public GoalFairyCount goalCount;
    private bool isMaster = false;

    // 인스펙터로 테스트를 한 후 해당 값을 Sprit의 private 변수로 변경하여 노출을 막아야함.
    public int tempFairyValue_1 = 10;
    public int tempFairyValue_2 = 10;
    public int tempFairyValue_3 = 10;

    // 채집물 분류에 따라 얻는 점수
    public int _FairyScore_1 = 1;
    public int _FairyScore_2 = 2;
    public int _FairyScore_3 = 3;
    
    //동연이꺼
    public string PlayAgainRoomCode { get; set; }
    public bool IsPlayAgainPending { get; set; }

    public static Manager Instance
    {
        get { return instance; }
    }

    public DataLoader dataLoader = new DataLoader();
    public IngameObserver observer = new IngameObserver();
    Shop shop;
    public List<Player> players = new List<Player>();
    public List<int> score = new List<int>();
    public List<int> deliveryCount = new List<int>();
    public List<int> hitCount = new List<int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        dataLoader.DataLoad();
    }

    private void Start()
    {
        //CheckData();
    }

    public void GetPlayerCount(int playercount)
    {
        observer.roomInPlayerCount = playercount;
    }

    public void CheckData()
    {
        for(int i = 0; i < dataLoader.data["SpaceShip"].Count; i++)
        {
            SpaceShip tempspaceship = new SpaceShip();
            tempspaceship.SpaceShipData = (SpaceShipData)dataLoader.data["SpaceShip"][i];
            Debug.Log(tempspaceship.SpaceShipData.Name);
        }
    }

    public void SetGameList()
    {
        Manager.instance.observer.UserPlayer.gamedata.score = 0;
        Manager.instance.observer.UserPlayer.gamedata.deliveryCount = 0;
        Manager.instance.observer.UserPlayer.gamedata.hitcount = 0;

        score.Clear();
        deliveryCount.Clear();
        hitCount.Clear();

        for( int i = 0; i < players.Count; i++)
        {
            if(players[i].ActorNumber == i+1)
            {
                score.Add(Manager.instance.observer.UserPlayer.gamedata.score);
                deliveryCount.Add(Manager.instance.observer.UserPlayer.gamedata.deliveryCount);
                hitCount.Add(Manager.instance.observer.UserPlayer.gamedata.hitcount);

                Debug.Log("스코어 기본 세팅" + score[0]);
                Debug.Log("반납 기본 세팅" + deliveryCount[0]);
                Debug.Log("히트 기본 세팅" + hitCount[0]);
            }
        }
    }
}

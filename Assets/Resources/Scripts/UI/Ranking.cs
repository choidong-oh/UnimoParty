using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Ranking : MonoBehaviour
{
    List<int> tempscore = new List<int>();
    List<int> ranks = new List<int>();
    public List<TextMeshProUGUI> scoretexts;
    public List<TextMeshProUGUI> nicknametexts;
    public List<TextMeshProUGUI> rewardtexts;
    public List<GameObject> rankObject;

    private void Awake()
    {
        Debug.Log("Awake 구독 확인 디버그 ");
        Manager.Instance.observer.OnGameEnd += GameEndResult;
    }

    private void Start()
    {
        Manager.Instance.observer.EndGame();
    }

    void GameEndResult()
    {
        Debug.Log("구독 확인 디버그");
        SetPlayerList();
        tempscore = Manager.Instance.score;
        DefaultPlayerScoreSetting(tempscore);
        RefreshPlayerScoreRank(tempscore);
        InitRankData();
    }

    void SetPlayerList()
    {
        for( int i = 1; i < Manager.Instance.players.Count; i++ )
        {
            rankObject[i+1].gameObject.SetActive(true);
        }
    }

    //버블소트를 통해 점수를 오름차순으로 정렬
    void DefaultPlayerScoreSetting(List<int> arr)
    {
        int n = arr.Count;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (arr[j] < arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
            }
        }

        Debug.Log($"정렬 결과: {{ {string.Join(", ", tempscore)} }}");
    }

    //1등과 점수를 비교하여, 등수를 부여
    void RefreshPlayerScoreRank(List<int> arr)
    {
        var temprank = 1;
        ranks.Clear();
        ranks.Add(temprank);

        for(int i = 0; i < arr.Count-1; i++)
        {
            if( arr[i] == arr[i + 1] )
            {
                ranks.Add(temprank);
            }
            else
            {
                temprank++;
                ranks.Add(temprank);
            }
        }
        Debug.Log($"정렬 결과: {{ {string.Join(", ", ranks)} }}");
    }

  
    void InitRankData()
    {
        for(int i = 0; i < Manager.Instance.players.Count;i++)
        {
            if(Manager.Instance.players[i].ActorNumber == i+1)
            {
                scoretexts[i].GetComponent<TextMeshProUGUI>().text = Manager.Instance.observer.UserPlayer.gamedata.score.ToString();
                nicknametexts[i].GetComponent<TextMeshProUGUI>().text = Manager.Instance.players[i].NickName.ToString();
            }
            

            Debug.Log(tempscore[i]);
        }   
    }
}
 
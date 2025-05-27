using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemySpawnBase : MonoBehaviour 
{
    public abstract void Spawn();
    public abstract void StopSpawnCor();

    [SerializeField] protected EnemySpawnerCommand enemySpawnerCommand; //생성커맨드패턴
    [SerializeField] protected Transform centerTransform; //맵중앙
    [SerializeField] protected Transform SpawnTransform;//스폰위치
    [SerializeField] protected List<Transform> players; //player위치
    [SerializeField] protected float distanceGap; //스폰위치 유저위치 사이의 거리 //플레이어위치에 안나오게
    [SerializeField] protected int r = 15; //반지름 //랜덤위치
    protected float angle;

    private void Start()
    {
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(p.transform);
        }

    }

    protected Vector3 DonutPostion()
    {
        //var tempPostion = new Vector3(20, 0, 16);

        var tempPostion = centerTransform.position;
        angle = UnityEngine.Random.Range(0, 360);
        float x = Mathf.Cos(angle) * r + tempPostion.x;
        float y = SpawnTransform.transform.position.y ;
        float z = Mathf.Sin(angle) * r + tempPostion.z;
        SpawnTransform.position = new Vector3(x, y, z);

        return SpawnTransform.position;
    }


    protected Vector3 isPlayerHere()
    {
        for (int i = 0; i < 5; i++)
        {
            //var dd = donutTransform.position; //테스트버전
            var dd = DonutPostion(); //실제
            bool isrmscj = false;
            foreach (var player in players)
            {
                if (Vector3.Distance(dd, player.position) < distanceGap)
                {
                    Debug.Log("플레이어가 근처임 ");
                    isrmscj = true; 
                    break;
                }
            }

            if (isrmscj == true)
            {

            }
            else
            {
                return dd;
            }
           
        }
        //나중에 player근처 아닐때까지 돌리면될듯
        Debug.Log("i번 돌렷는데 플레이어가 근처임");

        var aa = DonutPostion();
        return aa;
    }
    


}

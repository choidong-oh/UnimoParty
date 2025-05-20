using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//생성메서드
public class TestSpawnMgr : MonoBehaviour
{
    public EnemySpawnerCommand enemySpawnerCommand;

    void Start()
    {
        InvokeRepeating("SpawnRandomEnemy", 1f, 2f);
    }

    // 랜덤으로 적을 스폰하는 메서드
    void SpawnRandomEnemy()
    {
        string enemyType = "";

        if(Random.Range(0, 2) == 0)
        {
            enemyType = "Up";
        }
        else if(Random.Range (0, 2) == 1)
        {
            enemyType = "Right";
        }

        enemySpawnerCommand.SpawnEnemy(enemyType, Vector3.forward, 5);
    }




}

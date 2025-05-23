using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMgr : MonoBehaviour
{
    //0 : ¹øµÎ¸®
    [SerializeField] List<EnemySpawnBase> enemySpawnBase;

    private void Start()
    {
        //InvokeRepeating("AllSpawn", 1, 2);

        AllSpawn();
    }

    void AllSpawn()
    {
        enemySpawnBase[0].Spawn();
        enemySpawnBase[1].Spawn();
    }

}

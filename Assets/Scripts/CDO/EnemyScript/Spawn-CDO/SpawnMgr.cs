using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMgr : MonoBehaviour
{
    //0 : 번두리
    [SerializeField] List<EnemySpawnBase> enemySpawnBase;

    private void Start()
    {
        AllSpawn();
    }

    void AllSpawn()
    {
        enemySpawnBase[0].Spawn(); //번드리
        enemySpawnBase[1].Spawn(); //퓨퓨
        enemySpawnBase[2].Spawn(); //슉슉이
    }

    void StopAllCor()
    {
        enemySpawnBase[0].StopAllCoroutines();
        enemySpawnBase[1].StopAllCoroutines();
        enemySpawnBase[2].StopAllCoroutines();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//15rm 반지름안에 생성
//20초마다 1마리 생성
//기존 생생된 객체 사라지면 1마리 바로 생성
public class PewpewSpawn : EnemySpawnBase
{
    int EnemyMaxCount = 5;
    List<EnemyBase> pewpew =new List<EnemyBase>();

    public override void Spawn()
    {
        dsdsd();

    }

    void dsdsd()
    {
        StartCoroutine(SpawnCor());



    }

    IEnumerator SpawnCor()
    {
        while (true)
        {

                oneSpawn();
            for (int i = 0; i < EnemyMaxCount; i++)
            {
                pewpew.Add(enemySpawnerCommand.SpawnEnemy("Pewpew", isPlayerHere(), 5));
                yield return new WaitForSeconds(2);
            }
            yield return null;  

        }


    }

    void oneSpawn()
    {
        foreach (EnemyBase p in pewpew)
        {
            if (p.gameObject.activeInHierarchy == false)
            {
                pewpew.Add(enemySpawnerCommand.SpawnEnemy("Pewpew", isPlayerHere(), 5));
            }
        }
    }



}

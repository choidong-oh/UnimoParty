using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//번두리
// - 랜덤 생성 (r바꾸면댐)
// - 10초마다 1~2마리 생성
// - 최대 개체수
public  class BurnduriSpawn : EnemySpawnBase
{
    int enemyMaxCount = 5; //최대 몇 마리
    int currentEnemyCount = 0; //현재 몇마리 소환 됏는지
    float cycleSecond = 2f; //생성주기
    int maxRandomSpawnCount = 2; //한번에 랜덤으로 몇마리 소환하는지 
    LinkedList<EnemyBase> Burnduri = new LinkedList<EnemyBase>();

    public override void Spawn()
    {
        StartCoroutine(BurnduriSpawnInstantiateCor(cycleSecond));
        StartCoroutine(isSetactiveFalse());

    }
    public override void StopSpawnCor()
    {
        StopAllCoroutines();
    }

    IEnumerator BurnduriSpawnInstantiateCor(float CycleSecond)
    {
        while (true)
        {
            //현재 생성 수 == 최대 생성 마리수
            if (currentEnemyCount>=enemyMaxCount)
            {
               yield return null;
            }

            for (int i = 0; i < enemyMaxCount- currentEnemyCount; i++)
            {
                var randomInstantiateNum = Random.Range(0, maxRandomSpawnCount); //0~2

                Debug.Log(randomInstantiateNum);

                for (int j = 0; j <= randomInstantiateNum; j++)
                {
                    if (currentEnemyCount >= enemyMaxCount)
                    {
                        break;
                    }

                    currentEnemyCount++;
                    Burnduri.AddFirst(enemySpawnerCommand.SpawnEnemy("Burnduri", isPlayerHere(), 5));
                }


                yield return new WaitForSeconds(CycleSecond);
            }

            yield return null;
        }

    }


    IEnumerator isSetactiveFalse()
    {

        while (true)
        {
            var node = Burnduri.First;

            while (node != null)
            {
                var next = node.Next;
                //if (node.Value == null)
                if (node.Value.gameObject.activeInHierarchy == false)
                {
                    Burnduri.Remove(node);
                    currentEnemyCount--;
                }

                node = next;
            }

            yield return null;
        }


    }



}

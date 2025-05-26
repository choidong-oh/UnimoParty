using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//15rm 반지름안에 생성
//20초마다 1마리 생성
//기존 생생된 객체 사라지면 1마리 바로 생성
public class PewpewSpawn : EnemySpawnBase
{
    int enemyMaxCount = 5; //최대 몇 마리
    float cycleSecond = 20f; //생성주기
    LinkedList<EnemyBase> pewpew = new LinkedList<EnemyBase>();


    public override void Spawn()
    {
        StartCoroutine(PewpewSpawnInstantiateCor(cycleSecond));
        StartCoroutine(isSetactiveFalse());

    }



    IEnumerator PewpewSpawnInstantiateCor(float CycleSecond)
    {
        for (int i = 0; i < enemyMaxCount; i++)
        {
            pewpew.AddFirst(enemySpawnerCommand.SpawnEnemy("Pewpew", isPlayerHere(), 5));
            yield return new WaitForSeconds(CycleSecond);
        }
    }


    IEnumerator isSetactiveFalse()
    {

        while (true)
        {
            var node = pewpew.First;

            while (node != null)
            {
                var next = node.Next;

                if (node.Value.gameObject.activeInHierarchy == false)
                {
                    pewpew.Remove(node);
                    var enemy = enemySpawnerCommand.SpawnEnemy("Pewpew", isPlayerHere(), 5);
                    pewpew.AddAfter(pewpew.First, enemy);
                }

                node = next;
            }

            yield return null;
        }


    }

    void PewpewSpawnStopCor()
    {
        StopAllCoroutines();
    }

}

using UnityEngine;

public  class BurnduriSpawn : EnemySpawnBase
{
    public override void Spawn()
    {
        Debug.Log("BurnduriSpawn.spawn함수실행");
        //if (조건, 생성패턴) { }
        enemySpawnerCommand.SpawnEnemy("Burnduri", isPlayerHere(), 5);
    }

  
}

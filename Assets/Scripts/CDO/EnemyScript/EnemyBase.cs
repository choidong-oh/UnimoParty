using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    //csv
    //아래 변수 사용하세요~
    public string enemyName;              
    public float spawnStartTime;           
    public float spawnCycle;            
    public int damage;                     
    public float enemyMoveSpeed;                
    public int sizeScale;              
    public int spawnCount;                

    public abstract void Move(Vector3 direction);

    //base
    //public abstract void CsvEnemyInfo();
    public virtual void CsvEnemyInfo()
    {
        enemyName = "Burnduri";
        spawnStartTime = 0;
        spawnCycle = 20;
        damage = 1;
        enemyMoveSpeed = 0.5f;
        sizeScale = 1;//1~3
        spawnCount = 1;
    }
}


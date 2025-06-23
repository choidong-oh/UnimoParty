using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EnemyBase : MonoBehaviourPunCallbacks
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

    public bool ImFreeze { get;  set; }


    public abstract void Move(Vector3 direction);
    public abstract void Freeze(Vector3 direction, bool isFreeze);
    //base
    public abstract void CsvEnemyInfo();
   
}


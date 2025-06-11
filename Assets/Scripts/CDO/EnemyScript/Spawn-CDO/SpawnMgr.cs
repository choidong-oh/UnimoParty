using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnMgr : MonoBehaviour
{
    //0 : 번두리
    [SerializeField] List<EnemySpawnBase> enemySpawnBase;

    List<Laycock> Laycocks = new List<Laycock>();

    int LaycockCount = 0;

    public void LaycockCountCheck(int Count, Laycock Monster)
    {
        if (Count > 0)
        {
            Laycocks.Add(Monster);
        }
        else if (Count < 0)
        {
            Laycocks.Remove(Monster);
        }
        else
        {
            Debug.Log("스폰매니져 레이콕 쪽 오류");
        }
  
        LaycockCount = LaycockCount + Count;

        if (LaycockCount == 10)
        {
            foreach(Laycock laycock in Laycocks)
            {
                laycock.ShootLazer();
            }
        }
    }



    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            AllSpawn();
        }
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

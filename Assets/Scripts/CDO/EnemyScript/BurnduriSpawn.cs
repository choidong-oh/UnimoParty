using Unity.Mathematics;
using UnityEngine;

public class BurnduriSpawn : MonoBehaviour
{
    public EnemySpawnerCommand enemySpawnerCommand;
    public GameObject testPrefab;
    Vector3 spawnPosition;
    

    private void Start()
    {
        InvokeRepeating("Spawn", 1f, 2f);
        Spawn();
        Spawn();
        Spawn();
        Spawn();
        Spawn();
        Spawn();
        Spawn();
    }

    void SpawnOneEnemy()
    {
        spawnPosition = new Vector3(10,20,30);

        var pl =  Instantiate(testPrefab);
        pl.transform.position = GetRandomPosition(20);
        //오브, player 안되게
        //var tempEnemy = enemySpawnerCommand.SpawnEnemy("Burnduri", GetRandomPosition(20), 5);
        //if (tempEnemy.gameObject.activeInHierarchy)
        //{

        //}

    }


    void Spawn()
    {
        SpawnOneEnemy();
        SpawnOneEnemy();
        SpawnOneEnemy();
        SpawnOneEnemy();
        SpawnOneEnemy();
        SpawnOneEnemy();
        SpawnOneEnemy();
    }

    public Vector3 GetRandomPosition(float radius)
    {
        float x = UnityEngine.Random.Range(-1f, 1f);
        float temp = Mathf.Pow(1, 2) - Mathf.Pow(x, 2);
        float z = Mathf.Sqrt(temp);

        
        return (new Vector3(x,0,z)* UnityEngine.Random.Range(0,radius)
            + new Vector3(transform.position.x,0,transform.position.z));    
    }




}

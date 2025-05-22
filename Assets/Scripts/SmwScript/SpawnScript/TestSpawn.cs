using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    public int maxEnemies = 10;
    public int spawnTimer = 3;
    [SerializeField] GameObject Enemy;
    Vector3 spawnPos = new Vector3(25, 0, 25);

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }


    IEnumerator SpawnRoutine()
    {
        int spawned = 0;
        while (spawned < maxEnemies)
        {
            Instantiate(Enemy , spawnPos, Quaternion.identity);
            spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }
}

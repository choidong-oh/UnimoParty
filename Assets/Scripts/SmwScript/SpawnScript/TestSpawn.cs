using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    public float x = 25f;
    public float y = 25f;

    public int maxEnemies = 10;
    public int spawnTimer = 3;
    [SerializeField] GameObject Enemy;
    Vector3 spawnPos;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }


    IEnumerator SpawnRoutine()
    {
        int spawned = 0;
        while (spawned < maxEnemies)
        {
            spawnPos = new Vector3(x, 0, y);
            Instantiate(Enemy , spawnPos, Quaternion.identity);
            spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }
}

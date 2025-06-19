using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewPewSp : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    Terrain terrain;          

    [SerializeField] int maxEnemies = 10;
    [SerializeField] float spawnTimer = 3;

     Vector3 terrainCenter;


    private void Start()
    {
        terrain = Terrain.activeTerrain;
        terrainCenter = terrain.transform.position + terrain.terrainData.size * 0.5f;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        int spawned = 0;
        while (spawned < maxEnemies)
        {
            PoolManager.Instance.Spawn(enemyPrefab, terrainCenter, Quaternion.identity);
            spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    public void SpawnOne()
    {
        PoolManager.Instance.Spawn(enemyPrefab, terrainCenter, Quaternion.identity);
    }

}

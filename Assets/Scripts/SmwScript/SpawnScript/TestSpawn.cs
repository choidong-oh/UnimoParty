using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class TestSpawn : MonoBehaviour
{
    public float x = 25f;
    public float y = 25f;

    float RandomXMin;
    float RandomYMin;
    float RandomXMax;
    float RandomYMax;


    public int maxEnemies = 10;
    public int spawnTimer = 3;
    [SerializeField] GameObject[] Enemy;
    Vector3 spawnPos;

    Terrain terrain;

    private void Start()
    {
        terrain = Terrain.activeTerrain;
        Vector3 TerrainMin = terrain.transform.position;
        Vector3 TerrainMax = terrain.terrainData.size;

        RandomXMin = TerrainMin.x;
        RandomYMin = TerrainMin.z;

        RandomXMax = TerrainMin.x + TerrainMax.x;
        RandomYMax = TerrainMin.z + TerrainMax.z;

        StartCoroutine(SpawnRoutine());
    }


    IEnumerator SpawnRoutine()
    {
        
        int spawned = 0;
        while (spawned < maxEnemies)
        {
            Debug.Log("소환됨");

            //이거는 내가쓸 스포너여
            x = Random.Range(RandomXMin, RandomXMax);
            y = Random.Range(RandomYMin, RandomYMax);

            int RandomEnemy = Random.Range(0, Enemy.Length);
            spawnPos = new Vector3(x, 0, y);
            Instantiate(Enemy[RandomEnemy] , spawnPos, Quaternion.identity);
            spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }
}

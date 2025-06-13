using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnduriSP : MonoBehaviour
{
    float RandomXMin;
    float RandomZMin;
    float RandomXMax;
    float RandomZMax;

    [Header("스폰 설정")]
    [SerializeField] int maxEnemies = 10;
    [SerializeField] float spawnTimer = 3f;

    Vector3 spawnPos;
    Terrain terrain;

    [Header("가운데 비우기")]
    [SerializeField] float NoSpawn = 5f;
    [SerializeField] float SideNoSpawn;

    public static object BurnduriPool { get; private set; }

    private void Start()
    {
        terrain = Terrain.activeTerrain;

        Vector3 TerrainMin = terrain.transform.position;
        Vector3 TerrainMax = terrain.terrainData.size;

        RandomXMin = TerrainMin.x;
        RandomZMin = TerrainMin.z;

        RandomXMax = TerrainMin.x + TerrainMax.x;
        RandomZMax = TerrainMin.z + TerrainMax.z;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        int spawned = 0;

        float centerX = (RandomXMin + RandomXMax) * 0.5f;
        float centerZ = (RandomZMin + RandomZMax) * 0.5f;

        while (spawned < maxEnemies)
        {

            float RandomX = Random.Range(RandomXMin - SideNoSpawn, RandomXMax - SideNoSpawn);
            float RandomZ = Random.Range(RandomZMin - SideNoSpawn, RandomZMax - SideNoSpawn);
            while (Mathf.Abs(RandomX - centerX) < NoSpawn && Mathf.Abs(RandomZ - centerZ) < NoSpawn)
            {
                RandomX = Random.Range(RandomXMin - SideNoSpawn, RandomXMax - SideNoSpawn);
                RandomZ = Random.Range(RandomZMin - SideNoSpawn, RandomZMax - SideNoSpawn);
            }

 
            spawnPos = new Vector3(RandomX, 0f, RandomZ);


            //BurnduriPool.Instance.SpawnBurnduri(spawnPos, Quaternion.identity);

            spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }
}

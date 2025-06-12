using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class TestSpawn : MonoBehaviourPun
{
    float RandomXMin;
    float RandomZMin;
    float RandomXMax;
    float RandomZMax;

    [SerializeField] int maxEnemies = 10;
    [SerializeField] int spawnTimer = 3;
    [SerializeField] GameObject[] Enemy;

    Vector3 spawnPos;

    Terrain terrain;

    [SerializeField] float NoSpawn = 5f;
    [SerializeField] float SideNoSpawn;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

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

    [PunRPC]
    void SpawnRPC()
    {
        int RandomEnemy = Random.Range(0, Enemy.Length);

        Instantiate(Enemy[RandomEnemy], spawnPos, Quaternion.identity);
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

            //photonView.RPC("SpawnRPC", RpcTarget.All);
            int RandomEnemy = Random.Range(0, Enemy.Length);
            Instantiate(Enemy[RandomEnemy], spawnPos, Quaternion.identity);
            spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }
}

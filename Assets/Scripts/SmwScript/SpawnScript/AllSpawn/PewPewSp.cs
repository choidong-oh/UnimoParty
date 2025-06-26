//퓨퓨 스포너
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PewPewSp : MonoBehaviourPun
{
    [SerializeField] private string enemyPrefab = "Pewpew";

    Terrain terrain;

    [SerializeField] int maxEnemies = 5;
    [SerializeField] float spawnTimer = 3;

    Vector3 terrainCenter;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        terrain = Terrain.activeTerrain;
        terrainCenter = terrain.transform.position + terrain.terrainData.size * 0.5f;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        if (!PhotonNetwork.IsMasterClient) yield return null;
        int spawned = 0;
        Debug.Log("퓨퓨 스폰 " + spawned);
        while (spawned < maxEnemies)
        {
            PoolManager.Instance.SpawnNetworked(enemyPrefab, terrainCenter, Quaternion.identity);
            spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    public void SpawnOne()
    {
        Debug.Log("퓨퓨 재스폰");
        if (!PhotonNetwork.IsMasterClient) return;
        PoolManager.Instance.SpawnNetworked(enemyPrefab, terrainCenter, Quaternion.identity);

    }

}

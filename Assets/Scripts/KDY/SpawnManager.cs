using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [Header("스폰할 플레이어 프리팹")]
    public GameObject playerPrefab;

    [Header("스폰 포인트 리스트")]
    public List<Transform> spawnPoints = new List<Transform>();


    private void Start()
    {
        // 한명의 플레이어 스폰
        //SpawnAtIndex(0);

        // 8명 플레이어 스폰
        for(int i = 0; i < spawnPoints.Count; i++)
        {
            SpawnAtIndex(i);
        }
    }


    public void SpawnAtIndex(int index)
    {
        if (index >=0 &&  index < spawnPoints.Count)
        {
            Transform spawnPoint = spawnPoints[index];

            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            Debug.Log($"플레이어가 스폰됨: 인덱스 {index}, 위치:{spawnPoint.name}");
        }
        else
        {
            Debug.LogWarning($"SpawnAtIndex: 잘못된 인덱스 {index}");
        }
    }
    




}

using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("스폰할 플레이어 프리팹")]
    public GameObject playerPrefab;

    [Header("중심 오브젝트")]
    public Transform centerPoint;

    [Header("스폰 반지름 (중심에서 거리)")]
    public float spawnRadius = 5f;

    [Header("자동 생성된 스폰 포인트 (읽기 전용)")]
    public List<Transform> spawnPoints = new List<Transform>();

    private int currentSpawnIndex = 0;

    private void Start()
    {
        GenerateSpawnPointsAroundCenter();

        if (spawnPoints.Count > 0)
        {
            SpawnAtIndex(0); // 첫 번째 자동 생성된 위치에서 스폰
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnNext();
        }
    }

    private void SpawnNext()
    {
        currentSpawnIndex++;

        if (currentSpawnIndex >= spawnPoints.Count)
        {
            Debug.Log("더 이상 스폰할 위치가 없습니다.");
            return;
        }

        SpawnAtIndex(currentSpawnIndex);
    }

    public void SpawnAtIndex(int index)
    {
        if (index >= 0 && index < spawnPoints.Count)
        {
            Transform spawnPoint = spawnPoints[index];
            Vector3 spawnPos = spawnPoint.position;
            Quaternion yRotationOnly = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

            Instantiate(playerPrefab, spawnPos, yRotationOnly);
            Debug.Log($"플레이어가 스폰됨: 인덱스 {index}, 위치:{spawnPoint.position}");
        }
        else
        {
            Debug.LogWarning($"SpawnAtIndex: 잘못된 인덱스 {index}");
        }
    }

    private void GenerateSpawnPointsAroundCenter()
    {
        spawnPoints.Clear();

        if (centerPoint == null)
        {
            Debug.LogError("중심 오브젝트가 지정되지 않았습니다.");
            return;
        }

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * spawnRadius;
            float z = Mathf.Sin(angle) * spawnRadius;

            Vector3 spawnPos = centerPoint.position + new Vector3(x, 0f, z);

            // Terrain 높이 보정 (선택)
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                float terrainY = terrain.SampleHeight(spawnPos) + terrain.GetPosition().y;
                spawnPos.y = terrainY;
            }
            else
            {
                spawnPos.y = centerPoint.position.y;
            }

            // 빈 GameObject를 만들어서 위치 보관
            GameObject point = new GameObject($"SpawnPoint_{i}");
            point.transform.position = spawnPos;
            point.transform.SetParent(this.transform); // 계층 정리용
            spawnPoints.Add(point.transform);
        }

        Debug.Log("8개의 스폰 포인트가 자동 생성되었습니다.");
    }
}


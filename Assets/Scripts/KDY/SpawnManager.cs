//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.InputSystem; // New Input System 키보드 입력용
//using UnityEngine;

//// 스폰 포인트를 인덱스로 관리 및 스폰할 위치를 지정하는 스크립트
//// 인스펙터에서 스폰 포인트 리스트를 세팅하고,
//// 인덱스를 입력받아 해당 위치에 플레이어를 스폰하는 기능을 담당

//public class SpawnManager : MonoBehaviour
//{
//    // 스폰할 플레이어 프리팹을 인스펙터에서 연결
//    [Header("스폰할 플레이어 프리팹")]
//    public GameObject playerPrefab;

//    // 스폰 포인트들을 인스펙터에서 순선대로 할당 (원하는 규칙을 정해서 가능)
//    [Header("스폰 포인트 리스트")]
//    public List<Transform> spawnPoints = new List<Transform>();

//    private int currentSpawnIndex = 0; // 현재까지 스폰한 인덱스 번호

//    // 게임 시작시 테스트용으로 스폰실행 (삭제해도 상관없음)
//    private void Start()
//    {
//        // 첫 번째 스폰 위치에 한 명 스폰
//        SpawnAtIndex(0);  

//        // 모든 스폰 포인트에 순서대로 플레이어 스폰
//        //for (int i = 0; i < spawnPoints.Count; i++)
//        //{
//        //    SpawnAtIndex(i);      
//        //}
//    }

//    private void Update()
//    {
//        if (Keyboard.current.spaceKey.wasPressedThisFrame)
//        {
//            SpawnNext();
//        }
//    }

//    private void SpawnNext()
//    {
//        currentSpawnIndex++;

//        if (currentSpawnIndex >= spawnPoints.Count)
//        {
//            Debug.Log("더 이상 스폰할 위치가 없습니다.");
//            return;
//        }

//        SpawnAtIndex(currentSpawnIndex);
//    }



//    // 지정된 인덱스에 플레이어를 스폰하는 메서드
//    // index: 스폰할 위치의 인덱스 번호 (인스펙터에서 설정한 순서)
//    public void SpawnAtIndex(int index)
//    {
//        // 인덱스가 리스트 범위에 있는지 확인
//        if (index >=0 &&  index < spawnPoints.Count)
//        {
//            Transform spawnPoint = spawnPoints[index];

//            // 해당 스폰 포인트의 위치,회전을 가져와서 플레이어 생성
//            //Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

//            Vector3 spawnPos = spawnPoint.position;

//            // Y축 회전만 유지 (X/Z는 0으로)
//            Quaternion yRotationOnly = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

//            Instantiate(playerPrefab, spawnPos, yRotationOnly);

//            Debug.Log($"플레이어가 스폰됨: 인덱스 {index}, 위치:{spawnPoint.name}");
//        }
//        else
//        {
//            Debug.LogWarning($"SpawnAtIndex: 잘못된 인덱스 {index}");
//        }
//    }

//}

using System.Collections;
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


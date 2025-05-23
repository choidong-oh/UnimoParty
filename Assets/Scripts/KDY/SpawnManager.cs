using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //[Header("스폰할 플레이어 프리팹")]
    //public GameObject playerPrefab;

    [Header("스폰 포인트 리스트 (인스펙터에서 수동으로 설정)")]
    public List<Transform> spawnPoints = new List<Transform>();

    // 현재까지 스폰된 인덱스를 저장하는 변수
    private int currentSpawnIndex = 0;

    // 게임이 시작될 때 실행되는 함수
    private void Start()
    {
        // spawnPoints 리스트가 비어 있는 경우 경고 출력
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Spawn Points가 비어 있습니다. 인스펙터에서 수동으로 설정하세요.");
        }
        else
        {
            // 첫 번째 위치에 플레이어 스폰
            SpawnAtIndex(0);
        }
    }

    // 매 프레임마다 실행되는 함수
    private void Update()
    {
        // 스페이스 키를 누르면 다음 위치에 플레이어 스폰
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnNext();
        }
    }

    // 다음 인덱스로 플레이어를 스폰하는 함수
    private void SpawnNext()
    {
        currentSpawnIndex++;

        // 스폰 포인트를 모두 사용한 경우 로그 출력 후 종료
        if (currentSpawnIndex >= spawnPoints.Count)
        {
            Debug.Log("더 이상 스폰할 위치가 없습니다.");
            return;
        }

        // 다음 인덱스 위치에 스폰
        SpawnAtIndex(currentSpawnIndex);
    }

    // 특정 인덱스 위치에 플레이어를 스폰하는 함수
    public void SpawnAtIndex(int index)
    {
        // 유효한 인덱스인지 확인
        if (index >= 0 && index < spawnPoints.Count)
        {
            Transform spawnPoint = spawnPoints[index];
            Vector3 spawnPos = spawnPoint.position;

            // Y축 회전만 유지하고 나머지 회전은 제거
            Quaternion yRotationOnly = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

            // 플레이어 생성
            //Instantiate(playerPrefab, spawnPos, yRotationOnly);

            //Debug.Log($"플레이어가 스폰됨: 인덱스 {index}, 위치:{spawnPos}");
        }
        else
        {
            Debug.LogWarning($"SpawnAtIndex: 잘못된 인덱스 {index}");
        }
    }
}

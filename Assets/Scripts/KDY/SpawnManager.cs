using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스폰 포인트를 인덱스로 관리 및 스폰할 위치를 지정하는 스크립트
// 인스펙터에서 스폰 포인트 리스트를 세팅하고,
// 인덱스를 입력받아 해당 위치에 플레이어를 스폰하는 기능을 담당

public class SpawnManager : MonoBehaviour
{
    // 스폰할 플레이어 프리팹을 인스펙터에서 연결
    [Header("스폰할 플레이어 프리팹")]
    public GameObject playerPrefab;

    // 스폰 포인트들을 인스펙터에서 순선대로 할당 (원하는 규칙을 정해서 가능)
    [Header("스폰 포인트 리스트")]
    public List<Transform> spawnPoints = new List<Transform>();


    // 게임 시작시 테스트용으로 스폰실행 (삭제해도 상관없음)
    private void Start()
    {
        // 첫 번째 스폰 위치에 한 명 스폰
        SpawnAtIndex(0);  

        // 모든 스폰 포인트에 순서대로 플레이어 스폰
        //for (int i = 0; i < spawnPoints.Count; i++)
        //{
        //    SpawnAtIndex(i);      
        //}
    }

    // 지정된 인덱스에 플레이어를 스폰하는 메서드
    // index: 스폰할 위치의 인덱스 번호 (인스펙터에서 설정한 순서)
    public void SpawnAtIndex(int index)
    {
        // 인덱스가 리스트 범위에 있는지 확인
        if (index >=0 &&  index < spawnPoints.Count)
        {
            Transform spawnPoint = spawnPoints[index];

            // 해당 스폰 포인트의 위치,회전을 가져와서 플레이어 생성
            //Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

            Vector3 spawnPos = spawnPoint.position;

            // Y축 회전만 유지 (X/Z는 0으로)
            Quaternion yRotationOnly = Quaternion.Euler(0, spawnPoint.rotation.eulerAngles.y, 0);

            Instantiate(playerPrefab, spawnPos, yRotationOnly);

            Debug.Log($"플레이어가 스폰됨: 인덱스 {index}, 위치:{spawnPoint.name}");
        }
        else
        {
            Debug.LogWarning($"SpawnAtIndex: 잘못된 인덱스 {index}");
        }
    }
    




}

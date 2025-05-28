using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [Header("스폰 포인트 리스트 (인스펙터에서 수동으로 설정)")]
    public List<Transform> spawnPoints = new List<Transform>();

    // 현재까지 스폰된 인덱스를 저장하는 변수
    private int currentSpawnIndex = 0;

    // 게임이 시작될 때 실행되는 함수
    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsLocal)
        {
            SpawnAtIndex(PhotonNetwork.LocalPlayer.ActorNumber - 1);
        }
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
            PhotonNetwork.Instantiate("PlayerVariant", spawnPos, yRotationOnly);

            //Debug.Log($"플레이어가 스폰됨: 인덱스 {index}, 위치:{spawnPos}");
        }
        else
        {
            Debug.LogWarning($"SpawnAtIndex: 잘못된 인덱스 {index}");
        }
    }
}

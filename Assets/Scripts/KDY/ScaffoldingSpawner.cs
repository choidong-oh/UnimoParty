using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ScaffoldingSpawner : MonoBehaviour
{
    public GameObject[] flowerPrefabs; // 프리팹 3개 넣을 배열
    //public GameObject cubePrefab;    // 생성할 큐브 프리팹
    public Transform centerObject;     // 중심 기준이 되는 오브젝트

    private void Awake()
    {
        StartCoroutine(WaitForSceneChange());
    }
    IEnumerator WaitForSceneChange()
    {
        yield return new WaitForSeconds(0.5f);
    }
    void Start()
    {
        int gridSize = 6;                 // 격자 크기 6x6
        float area = 80f;                 // 사용할 맵 영역 크기
        float spacing = area / gridSize;  // 큐브 간의 간격

        Vector3 center = centerObject.position;           // 기준 중심 위치
        
        int prefabIndex = 0;

        for (int x = 0; x < gridSize; x++)                // X 방향 루프
        {
            for (int z = 0; z < gridSize; z++)            // Z 방향 루프
            {
                // 중앙 AUBE 자리(2,2), (2,3), (3,2), (3,3)는 건너뛴다
                if ((x == 2 || x == 3) && (z == 2 || z == 3))
                    continue;

                // 중심 기준으로 위치 계산
                float offsetX = (x - 2.5f) * spacing;
                float offsetZ = (z - 2.5f) * spacing;
                Vector3 spawnPos = center + new Vector3(offsetX, 0f, offsetZ);

                // 큐브 생성
                if (PhotonNetwork.IsMasterClient)
                {
                    //GameObject cube = PhotonNetwork.Instantiate("Flower", spawnPos, Quaternion.identity);

                    GameObject prefab = flowerPrefabs[prefabIndex % flowerPrefabs.Length];
                    GameObject flower = PhotonNetwork.Instantiate(prefab.name, spawnPos, Quaternion.identity);
                    prefabIndex++;
                    Renderer rend = flower.GetComponent<Renderer>();
                    if (rend != null)
                        rend.material.color = new Color(Random.value, Random.value, Random.value);
                }

                // 랜덤 색상 적용
            }
        }
    }
}

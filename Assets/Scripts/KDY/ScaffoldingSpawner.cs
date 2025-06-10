//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ScaffoldingSpawner : MonoBehaviour
//{
//    public GameObject cubePrefab;     // 하나 만든 큐브 프리팹
//    public Transform centerObject;    // 중앙 기준 오브젝트
//    public int cubeCount = 10;

//    void Start()
//    {
//        for (int i = 0; i < cubeCount; i++)
//        {
//            // 큐브 생성
//            GameObject cube = Instantiate(cubePrefab);

//            // 랜덤 위치 (중앙 기준)
//            Vector3 offset = new Vector3(
//                Random.Range(-18f, 18f),
//                0f,
//                Random.Range(-18f, 18f)
//            );

//            Vector3 spawnPos = centerObject.position + offset;

//            // Terrain 높이 보정
//            Terrain terrain = Terrain.activeTerrain;
//            if (terrain != null)
//            {
//                float terrainY = terrain.SampleHeight(spawnPos) + terrain.GetPosition().y;
//                spawnPos.y = terrainY;
//            }

//            cube.transform.position = spawnPos;

//            // 랜덤 색상 적용
//            Renderer rend = cube.GetComponent<Renderer>();
//            if (rend != null)
//                rend.material.color = new Color(Random.value, Random.value, Random.value);
//        }
//    }
//}

using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ScaffoldingSpawner : MonoBehaviour
{
    public GameObject cubePrefab;         // 생성할 큐브 프리팹
    public Transform centerObject;        // 중심 기준이 되는 오브젝트

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
        int gridSize = 6;                 // 격자 크기: 6x6
        float area = 80f;                 // 사용할 맵 영역 크기 (터레인은 100x100이지만 80x80 기준으로 제한)
        float spacing = area / gridSize;  // 각 큐브 간의 간격

        Vector3 center = centerObject.position;           // 기준 중심 위치
        //Terrain terrain = Terrain.activeTerrain;          // Terrain 참조

        for (int x = 0; x < gridSize; x++)                // X 방향 루프
        {
            for (int z = 0; z < gridSize; z++)            // Z 방향 루프
            {
                // 중앙 AUBE 자리(2,2), (2,3), (3,2), (3,3)는 건너뛴다
                if ((x == 2 || x == 3) && (z == 2 || z == 3))
                    continue;

                // 중심 기준으로 위치 오프셋 계산
                float offsetX = (x - 2.5f) * spacing;
                float offsetZ = (z - 2.5f) * spacing;
                Vector3 spawnPos = center + new Vector3(offsetX, 0f, offsetZ);

                // Terrain이 있다면 해당 위치의 높이 적용
                //if (terrain != null)
                //{
                //   float terrainY = terrain.SampleHeight(spawnPos) + terrain.GetPosition().y;
                //  spawnPos.y = terrainY;
                //}

                // 큐브 생성
                if (PhotonNetwork.IsMasterClient)
                {
                    GameObject cube = PhotonNetwork.Instantiate("Flower", spawnPos, Quaternion.identity);
                    Renderer rend = cube.GetComponent<Renderer>();
                    if (rend != null)
                        rend.material.color = new Color(Random.value, Random.value, Random.value);
                }

                // 랜덤 색상 적용
            }
        }
    }
}

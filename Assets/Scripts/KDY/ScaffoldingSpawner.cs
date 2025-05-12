using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaffoldingSpawner : MonoBehaviour
{
    public GameObject cubePrefab;     // 하나 만든 큐브 프리팹
    public Transform centerObject;    // 중앙 기준 오브젝트
    public int cubeCount = 10;

    void Start()
    {
        for (int i = 0; i < cubeCount; i++)
        {
            // 큐브 생성
            GameObject cube = Instantiate(cubePrefab);

            // 랜덤 위치 (중앙 기준)
            Vector3 offset = new Vector3(
                Random.Range(-120f, 120f),
                0f,
                Random.Range(-120f, 120f)
            );
            cube.transform.position = centerObject.position + offset;

            // 랜덤 색상 적용
            Renderer rend = cube.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }
}

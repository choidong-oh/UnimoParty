using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDonutSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] GameObject enemyPrefab;//적 프리팹
    [SerializeField] int maxEnemies = 10; //적 최대숫자 
    [SerializeField] float spawnTimer = 20f; //적 다음 스폰시간 

    [Header("도넛 영역")]
    [SerializeField] float innerRadius = 5f; //중앙 스폰금지 지역
    [SerializeField] float outerRadius = 20f;//전체 스폰지역 

    [Header("디버그")]
    [SerializeField] bool showGizmos = true;// 이거는 기즈모를 보여줄꺼냐 말꺼냐 개발자전용

    private int currentEnemyCount = 0; // 이거는 적 갯수를 세주는 것
    private List<Vector3> spawnPositions = new List<Vector3>();//이거는 기즈모로 처음 스폰을 어디서 했는지 몰려고 만든거 

    void Start()
    {
        // 최초에 maxEnemies만큼 스폰
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnOneEnemy();
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    // 적 1마리 스폰 함수
    void SpawnOneEnemy()
    {
        Vector3 spawnPos = GetDonutSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        spawnPositions.Add(spawnPos);
        currentEnemyCount++;

        // 적에게 스폰어 정보 전달
        pupu enemyScript = enemy.GetComponent<pupu>();
        if (enemyScript != null)
            enemyScript.spawner = this;
    }

    // 적이 사라질 때 호출되는 함수
    public void OnEnemyRemoved()
    {
        currentEnemyCount--;

        if (isQuitting) return; // 종료중이면 더 이상 스폰X

        if (currentEnemyCount < maxEnemies)
            SpawnOneEnemy();
    }

    private bool isQuitting = false;

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    // 도넛 영역 랜덤 위치 반환
    Vector3 GetDonutSpawnPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(innerRadius, outerRadius);
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 center = transform.position;
        return new Vector3(center.x + x, 0, center.z + z); 
    }

    // 도넛 영역 + 스폰 지점 기즈모 표시
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // 도넛 외곽선
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        // 도넛 내부(구멍)
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        // 스폰된 지점
        Gizmos.color = Color.white;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawSphere(pos, 0.5f);
        }
    }
}

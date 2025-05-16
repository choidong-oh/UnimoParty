using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDonutSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxEnemies = 10;
    [SerializeField] float spawnTimer = 20f;

    [Header("도넛 영역")]
    [SerializeField] float innerRadius = 5f;
    [SerializeField] float outerRadius = 20f;
    [SerializeField] float spawnY = 0.5f;

    [Header("디버그")]
    [SerializeField] bool showGizmos = true;

    private int currentEnemyCount = 0;
    private List<Vector3> spawnPositions = new List<Vector3>();

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
        return new Vector3(center.x + x, spawnY, center.z + z);
    }

    // 도넛 영역 + 스폰 지점 기즈모 표시
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // 도넛 외곽선
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        // 도넛 내부(구멍)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        // 스폰된 지점
        Gizmos.color = Color.yellow;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawSphere(pos, 0.5f);
        }
    }
}

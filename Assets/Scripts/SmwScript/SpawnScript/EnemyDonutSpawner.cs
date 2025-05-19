using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDonutSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxEnemies = 10;
    [SerializeField] float spawnTimer = 20f;
    [SerializeField] int poolSize = 20; // 풀 크기

    [Header("도넛 영역")]
    [SerializeField] float innerRadius = 5f;
    [SerializeField] float outerRadius = 20f;

    [Header("디버그")]
    [SerializeField] bool showGizmos = true;

    private List<GameObject> enemyPool = new List<GameObject>();
    private int currentEnemyCount = 0;
    private bool isQuitting = false;
    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        // 1. 풀 미리 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, Vector3.one * 9999, Quaternion.identity);
            obj.SetActive(false);
            enemyPool.Add(obj);

            // 스포너 참조 넘기기
            pupu enemyScript = obj.GetComponent<pupu>();
            if (enemyScript != null)
                enemyScript.spawner = this;
        }

        // 2. 최초 maxEnemies만큼 스폰
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        int spawned = 0;
        while (spawned < maxEnemies)
        {
            if (SpawnOneEnemy())
                spawned++;
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    // 풀에서 꺼내서 스폰
    bool SpawnOneEnemy()
    {
        GameObject enemy = GetPooledEnemy();
        if (enemy == null)
            return false; // 풀에 여유 없음

        Vector3 spawnPos = GetDonutSpawnPosition();
        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;
        enemy.SetActive(true);

        spawnPositions.Add(spawnPos);
        currentEnemyCount++;
        return true;
    }

    // 비활성화된 오브젝트 반환
    GameObject GetPooledEnemy()
    {
        foreach (var obj in enemyPool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        return null;
    }

    // 적이 비활성화(죽음 등)될 때 호출
    public void OnEnemyRemoved()
    {
        currentEnemyCount--;
        if (isQuitting) return;

        if (currentEnemyCount < maxEnemies)
            StartCoroutine(DelayedSpawn());
    }

    IEnumerator DelayedSpawn()
    {
        yield return null; // 다음 프레임까지 대기
        SpawnOneEnemy();
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    Vector3 GetDonutSpawnPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(innerRadius, outerRadius);
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 center = transform.position;
        return new Vector3(center.x + x, 0, center.z + z);
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.yellow;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawSphere(pos, 0.5f);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllSpawn : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxEnemies = 10;
    [SerializeField] float spawnTimer = 3f;
    [SerializeField] int poolSize = 20;
    [SerializeField] int minSpawn = 1;      // 한 번에 최소 스폰 수
    [SerializeField] int maxSpawn = 4;      // 한 번에 최대 스폰 수 (n 값)


    [Header("박스 영역")]
    [SerializeField] float rectWidth = 30f;
    [SerializeField] float rectHeight = 30f;

    [Header("금지 구역(중앙 원형)")]
    [SerializeField] float innerRadius = 5f; // 중심에서 이 거리 이내엔 스폰X

    [Header("디버그")]
    [SerializeField] bool showGizmos = true;

    private List<GameObject> enemyPool = new List<GameObject>();
    private bool isQuitting = false;

    // 스폰 위치 기록용 (기즈모 표시용)
    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        // 풀 미리 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, Vector3.one * 9999, Quaternion.identity);
            obj.SetActive(false);
            enemyPool.Add(obj);
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            int activeCount = GetActiveEnemyCount();
            int spawnable = maxEnemies - activeCount;
            if (spawnable > 0)
            {
                int wantToSpawn = Random.Range(minSpawn, maxSpawn + 1);

                // 비활성화된 적의 수
                int available = 0;
                foreach (var obj in enemyPool)
                    if (!obj.activeInHierarchy) available++;

                int toSpawn = Mathf.Min(wantToSpawn, spawnable, available);

                for (int i = 0; i < toSpawn; i++)
                {
                    SpawnOneEnemy();
                }
            }
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    void SpawnOneEnemy()
    {
        GameObject enemy = GetPooledEnemy();
        if (enemy == null) return; // 풀에 여유 없음

        Vector3 spawnPos = GetBoxDonutSpawnPosition();
        enemy.transform.position = spawnPos;
        enemy.transform.rotation = Quaternion.identity;
        enemy.SetActive(true);

        spawnPositions.Add(spawnPos);
    }

    GameObject GetPooledEnemy()
    {
        foreach (var obj in enemyPool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        return null; // 모두 사용중
    }

    int GetActiveEnemyCount()
    {
        int count = 0;
        foreach (var obj in enemyPool)
        {
            if (obj.activeInHierarchy)
                count++;
        }
        return count;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    // 사각형 도넛: 중심에서 innerRadius 이내는 금지
    Vector3 GetBoxDonutSpawnPosition()
    {
        Vector3 center = transform.position;
        float halfWidth = rectWidth / 2f;
        float halfHeight = rectHeight / 2f;
        Vector3 pos;

        int safety = 0;
        do
        {
            float x = Random.Range(center.x - halfWidth, center.x + halfWidth);
            float z = Random.Range(center.z - halfHeight, center.z + halfHeight);
            pos = new Vector3(x, 0, z);
            if (++safety > 20) break;
        } while (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(center.x, center.z)) < innerRadius);

        return pos;
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // 박스 외곽선
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(rectWidth, 0.1f, rectHeight));

        // 금지 원형 (중앙)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        // 스폰 위치(노랑)
        Gizmos.color = Color.yellow;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawSphere(pos + Vector3.up * 0.2f, 0.5f);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllSpawn : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxEnemies = 10;
    [SerializeField] float spawnInterval = 3f;

    [Header("박스 영역")]
    [SerializeField] float rectWidth = 30f;
    [SerializeField] float rectHeight = 30f;

    [Header("금지 구역(중앙 원형)")]
    [SerializeField] float innerRadius = 5f; // 중심에서 이 거리 이내엔 스폰X

    [Header("디버그")]
    [SerializeField] bool showGizmos = true;

    private int currentEnemyCount = 0;
    private bool isQuitting = false;

    // 스폰 위치 기록용 (기즈모 표시용)
    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            int spawnable = maxEnemies - currentEnemyCount;
            if (spawnable > 0)
            {
                int toSpawn = Mathf.Min(Random.Range(1, 3), spawnable);
                for (int i = 0; i < toSpawn; i++)
                {
                    SpawnOneEnemy();
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneEnemy()
    {
        Vector3 spawnPos = GetBoxDonutSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentEnemyCount++;

        // 스폰 위치 저장(기즈모 표시용)
        spawnPositions.Add(spawnPos);

    }

    public void OnEnemyRemoved()
    {
        currentEnemyCount--;
        if (isQuitting) return;
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
            if (++safety > 20) break; // 무한루프 방지
        } while (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(center.x, center.z)) < innerRadius);

        return pos;
    }

    // 기즈모
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
        Gizmos.color = Color.red;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawSphere(pos + Vector3.up * 0.2f, 0.5f);
        }
    }
}

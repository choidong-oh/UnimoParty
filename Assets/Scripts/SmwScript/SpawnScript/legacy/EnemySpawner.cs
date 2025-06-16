using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("기본 설정")]
    Transform SpawerCenter;//스포너 중심    

    [SerializeField] GameObject enemyPrefab;//적 프리팹 넣기 
    [SerializeField] float spawnRadius = 20f;//생성범위

    [Header("스폰 설정")]
    [SerializeField] float SpawnTimer = 3f;//스폰 시간 지금은 3초
    [SerializeField] int maxEnemies = 10; //적 최대 숫자

    [Header("디버그 표시")]
    [SerializeField] bool showGizmos = true;// 기즈모 보이게 할껀지

    private int currentEnemyCount = 0;//생성 숫자 샐 변수
    private Vector3[] lastSpawnPositions;

    void Start()
    {
        SpawerCenter = transform;

        lastSpawnPositions = new Vector3[maxEnemies];
        StartCoroutine(SpawnEnemiesRoutine());
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (currentEnemyCount < maxEnemies)
        {
            Vector3 spawnPos = SpawnEnemyOnHorizontalEdge();

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, transform.rotation);
            MoveForward move = enemy.GetComponent<MoveForward>();
            if (move != null)
            {
                move.Benchmark(SpawerCenter); //원의중심을 적에게 알려줌
            }
            lastSpawnPositions[currentEnemyCount] = spawnPos;
            currentEnemyCount++;

            yield return new WaitForSeconds(SpawnTimer);
        }
    }

    // ▶︎ 수평 방향 벡터 반환
    Vector3 GetHorizontalDirection()
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        return new Vector3(dir2D.x, 0f, dir2D.y);
    }

    // ▶︎ 정확히 Y = 0.5에 생성
    Vector3 SpawnEnemyOnHorizontalEdge()
    {
        Vector3 direction = GetHorizontalDirection();
        Vector3 spawnPos = SpawerCenter.position + direction * spawnRadius;

        return spawnPos;
    }



    // ▶︎ Gizmo 표시 (디버그용)
    void OnDrawGizmosSelected()
    {
        if (!showGizmos || SpawerCenter == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(SpawerCenter.position, spawnRadius);

        if (lastSpawnPositions != null)
        {
            foreach (var pos in lastSpawnPositions)
            {
                if (pos == Vector3.zero) continue;

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(pos, 0.5f);
            }
        }
    }
}



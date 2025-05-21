using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;

public class AllSpawn1 : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxEnemies = 10;
    [SerializeField] float spawnTimer = 3f;
    [SerializeField] int minSpawn = 1;
    [SerializeField] int maxSpawn = 4;

    [Header("박스 영역")]
    [SerializeField] float rectWidth = 30f;
    [SerializeField] float rectHeight = 30f;

    [Header("금지 구역(중앙 원형)")]
    [SerializeField] float innerRadius = 5f;

    [Header("디버그")]
    [SerializeField] bool showGizmos = true;

    private List<EnemyBase> activeEnemies = new List<EnemyBase>();
    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            CleanDeadEnemies();

            int spawnable = maxEnemies - activeEnemies.Count;
            if (spawnable > 0)
            {
                int wantToSpawn = Random.Range(minSpawn, maxSpawn + 1);
                int toSpawn = Mathf.Min(wantToSpawn, spawnable);

                for (int i = 0; i < toSpawn; i++)
                {
                    SpawnOneEnemy();
                }
            }
            yield return new WaitForSeconds(spawnTimer);
        }

    }

    public EnemySpawnerCommand enemySpawnerCommand;
    void SpawnOneEnemy()
    {
        Vector3 spawnPos = GetBoxDonutSpawnPosition();
        //GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        var tempEnemy = enemySpawnerCommand.SpawnEnemy("Burnduri", GetBoxDonutSpawnPosition(), 5);
        activeEnemies.Add(tempEnemy);
        spawnPositions.Add(spawnPos);


    }

    void CleanDeadEnemies()
    {
        activeEnemies.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);
    }

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

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(rectWidth, 0.1f, rectHeight));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.yellow;
        foreach (var pos in spawnPositions)
        {
            Gizmos.DrawSphere(pos + Vector3.up * 0.2f, 0.5f);
        }
    }
}

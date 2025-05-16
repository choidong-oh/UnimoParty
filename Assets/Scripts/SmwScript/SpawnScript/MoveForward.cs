using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f; // 앞으로 나가는 스피드
    [SerializeField] float fixedY = -0.5f;  // 바닥에서 띄울 높이
    [SerializeField] float DirectionAngle = 90; // 방향 퍼짐 각도

    float pointRecordInterval = 0.2f; // 경로 기록 간격
    Transform Spwner;                 // 원 중심 기준점
    int EnemytypeNum;                 // 적 타입 번호

    private Collider myCollider;       // 자기 자신의 콜라이더 저장
    private List<Vector3> pathPoints = new List<Vector3>();

    public void Benchmark(Transform SpawnerTransform) // 기준점 받아오기
    {
        Spwner = SpawnerTransform;
    }

    public void SetEnemytype(int Enemytype) // 몬스터 종류 세팅
    {
        EnemytypeNum = Enemytype;
    }

    void Start()
    {
        myCollider = GetComponent<Collider>(); // 내 콜라이더 미리 저장

        // 시작 위치 기록 및 높이 고정
        Vector3 startPos = transform.position;
        startPos.y = fixedY;
        pathPoints.Add(startPos);

        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;

        SetInitialDirection();

        if (EnemytypeNum == 0)
            StartCoroutine(MoveForwardRoutine());

        StartCoroutine(RecordPathRoutine());
        StartCoroutine(LifeTime(30f));
    }

    void SetInitialDirection()
    {
        if (Spwner == null) return;

        Vector3 toCenter = Spwner.position - transform.position;
        toCenter.y = 0f;
        toCenter.Normalize();

        float angleOffset = Random.Range(-DirectionAngle / 2, DirectionAngle / 2);
        Vector3 offsetDir = Quaternion.Euler(0f, angleOffset, 0f) * toCenter;

        transform.rotation = Quaternion.LookRotation(offsetDir);
    }

    IEnumerator MoveForwardRoutine()
    {
        while (true)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            Vector3 pos = transform.position;
            float terrainY = Terrain.activeTerrain.SampleHeight(pos);
            pos.y = terrainY + fixedY;
            transform.position = pos;

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator LifeTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    IEnumerator RecordPathRoutine()
    {
        while (true)
        {
            Vector3 point = transform.position;
            point.y = fixedY;
            pathPoints.Add(point);
            yield return new WaitForSeconds(pointRecordInterval);
        }
    }

    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Count < 2) return;

        Gizmos.color = Color.blue;
        for (int i = 1; i < pathPoints.Count; i++)
        {
            Gizmos.DrawLine(pathPoints[i - 1], pathPoints[i]);
        }
    }
}

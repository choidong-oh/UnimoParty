using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveForward : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;// 말그대로 앞으로 나가는 스피드
    [SerializeField] float fixedY = 0.5f;// 탄막 높이설정(탄막이 크면 그만큼 올려야함)
    [SerializeField] float DirectionAngle = 90;// max는 360도임 그이상하지 않기 


    float pointRecordInterval = 0.2f;   // 몇 초마다 경로 기록할지

    Transform Spwner;// 이거는 원중심을 찾기위해서(기준점)
    int EnemytypeNum;

    public void Benchmark(Transform SpawnerTransform) //값을 받아올꺼임 여기서 
    {
        Spwner = SpawnerTransform;
    }


    public void SetEnemytype(int Enemytype) //몬스터 종류 숫자넣어주면 여기서 그몬스터를 소환함
    {
        EnemytypeNum = Enemytype;
    }

    private List<Vector3> pathPoints = new List<Vector3>();//기즈모 기록을 여기서 담을꺼임

    void Start()
    {
        // 시작 위치 기록
        Vector3 startPos = transform.position;
        startPos.y = fixedY;
        pathPoints.Add(startPos);

        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;


        SetInitialDirection();//이거는 원안으로 쏘게 방향을 정해줌

        if (EnemytypeNum == 0)//여기서 들어간 프리팹의 패턴을 넣어줌 
        {
            StartCoroutine(MoveForwardRoutine());//이게 실제로 움직임
        }

        StartCoroutine(RecordPathRoutine());//이거는 기즈모로 경로 확인 코루틴 
    }

    void SetInitialDirection()
    {
        if (Spwner == null) return;

        Vector3 toCenter = Spwner.position - transform.position;

        //Y축 제거해서 평면 방향으로 만듦
        toCenter.y = 0f;
        toCenter.Normalize();

        float angleOffset = Random.Range(-DirectionAngle / 2, DirectionAngle / 2);//여기서 각도 설정
        Vector3 offsetDir = Quaternion.Euler(0f, angleOffset, 0f) * toCenter;

        transform.rotation = Quaternion.LookRotation(offsetDir);


    }


    IEnumerator MoveForwardRoutine()//실제 움직임 이런걸 종류를 늘리면됨 (패턴 추가할시)
    {
        while (true)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }




    IEnumerator RecordPathRoutine()//기즈모기록 
    {
        while (true)
        {
            Vector3 point = transform.position;
            point.y = fixedY;
            pathPoints.Add(point);
            yield return new WaitForSeconds(pointRecordInterval);
        }
    }

    void OnDrawGizmos()//기즈모 그리기
    {
        if (pathPoints == null || pathPoints.Count < 2) return;

        Gizmos.color = Color.blue;
        for (int i = 1; i < pathPoints.Count; i++)
        {
            Gizmos.DrawLine(pathPoints[i - 1], pathPoints[i]);
        }
    }
}

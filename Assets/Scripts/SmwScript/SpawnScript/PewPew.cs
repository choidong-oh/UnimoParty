using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PewPew : EnemyBase
{
    Vector3 Position;

    float Radius;
    float MoveSpeed = 10;
    float Angle;           

    int rotateDirection;   //어디로갈지 시계 반시계
    float fixedY = 0f;// 나중에 조절하게 만들꺼

    Coroutine rotateCoroutine;

    Terrain terrain;

    private void OnEnable()
    {
        // 1. Terrain 참조
        terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogWarning("트레인 없다 트레인쓰세요.");
        }
        else
        {
            Vector3 tPos = terrain.transform.position;
            Vector3 tSize = terrain.terrainData.size;

            float centerX = tPos.x + tSize.x * 0.5f;
            float centerZ = tPos.z + tSize.z * 0.5f;

            Position = new Vector3(centerX, 0, centerZ);//트레인기준 중심
        }

        //랜덤 몬스터 크기
        float RandomScale = Random.Range(1, 4) * 0.3f;
        transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);

        //랜덤 각도에서 시작
        Angle = Random.Range(0f, Mathf.PI * 2f);
        //랜덤반지름 위치 
        Radius = Random.Range(3f, 20f);
        //랜덤 회전 방향(1 or -1)
        rotateDirection = Random.value < 0.5f ? 1 : -1;
        rotateCoroutine = StartCoroutine(GoPewPew());//굳이 변수 선언한건 값 초기화 때문
    }


    // 오브젝트가 꺼질 때 코루틴 정리
    void OnDisable()
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }

    }

    IEnumerator GoPewPew()
    {
        while (true)
        {
            float angularSpeed =  MoveSpeed / Radius; //원 둘레를 도는 속도
            Angle -= angularSpeed * Time.deltaTime * rotateDirection;//각도를 회전 방향에 따라 바꿔줌

            //위치 계산해서 이동
            float x = Position.x + Mathf.Cos(Angle) * Radius;
            float z = Position.z + Mathf.Sin(Angle) * Radius;

            float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;
            Vector3 newPos = new Vector3(x, terrainY, z);

            Vector3 moveDir = (newPos - transform.position).normalized;
            if (moveDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
            }

            transform.position = newPos;

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            damage = 1;
            //Manager.Instance.observer.HitPlayer(damage);
            //Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

            gameObject.SetActive(false);
        }

    }

    public override void Move(Vector3 direction)
    {
        // 1. Terrain 참조
        terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogWarning("트레인 없다 트레인쓰세요.");
        }
        else
        {
            Vector3 tPos = terrain.transform.position;
            Vector3 tSize = terrain.terrainData.size;

            float centerX = tPos.x + tSize.x * 0.5f;
            float centerZ = tPos.z + tSize.z * 0.5f;

            Position = new Vector3(centerX, 0, centerZ);//트레인기준 중심
        }

        //랜덤 몬스터 크기
        float RandomScale = Random.Range(1, 4) * 0.3f;
        transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);

        //랜덤 각도에서 시작
        Angle = Random.Range(0f, Mathf.PI * 2f);
        //랜덤반지름 위치 
        Radius = Random.Range(3f, 20f);
        //랜덤 회전 방향(1 or -1)
        rotateDirection = Random.value < 0.5f ? 1 : -1;
        rotateCoroutine = StartCoroutine(GoPewPew());//굳이 변수 선언한건 값 초기화 때문
    }

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }
}

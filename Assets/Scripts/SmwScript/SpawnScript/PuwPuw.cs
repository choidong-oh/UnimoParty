using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuwPuw : EnemyBase
{
    Vector3 Position;

    float Radius;
    float MoveSpeed=10;
    float Angle;           
    int rotateDirection;   //어디로갈지 시계 반시계
    float fixedY = 0f;// 나중에 조절하게 만들꺼

    Coroutine rotateCoroutine;

    private void OnEnable()
    {
        Position = transform.position;
        //랜덤 몬스터 크기
        int RandomScale = Random.Range(1, 4);
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
            StopCoroutine(rotateCoroutine);
        rotateCoroutine = null;
    }

    IEnumerator GoPewPew()
    {
        while (true)
        {

            //원 둘레를 도는 속도
            float angularSpeed =  MoveSpeed / Radius;

            //각도를 회전 방향에 따라 바꿔줌
            Angle -= angularSpeed * Time.deltaTime * rotateDirection;

            //위치 계산해서 이동
            float x = Position.x + Mathf.Cos(Angle) * Radius;
            float z = Position.z + Mathf.Sin(Angle) * Radius;

            //여기서 Y값만 Terrain 높이로 교체
            float terrainY = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
            terrainY += transform.localScale.y / 2f;

            // Y값만 Terrain 높이로 적용
            transform.position = new Vector3(x, terrainY + fixedY, z);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            damage = 1;
            Manager.Instance.observer.HitPlayer(damage);
            //Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
        }

    }

    public override void Move(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }
}

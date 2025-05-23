using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuwPuw : EnemyBase
{
    Vector3 Postion;

    float Radius;
    float fixedY;
    float MoveSpeed=10;
    float Angle;           
    int rotateDirection;   //어디로갈지 시계 반시계

    Coroutine rotateCoroutine;

    Transform center;

    private void Start()
    {
        //center.position = Vector3.zero;
    }

    public override void Move(Vector3 direction)
    {
        Postion = transform.position;

        int RandomScale = Random.Range(1, 4);
        transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
        fixedY = RandomScale / 2f;

        //랜덤 각도에서 시작
        Angle = Random.Range(0f, Mathf.PI * 2f);

        //반지름 위치 
        Radius = Random.Range(3f, 20f);

        //랜덤 회전 방향(1 or -1)
        rotateDirection = Random.value < 0.5f ? 1 : -1;

        //원을 따라 도는 코루틴 시작
        rotateCoroutine = StartCoroutine(GoPewPew());//굳이 변수 선언한건 값 초기화 때문
    }

    public override void CsvEnemyInfo()
    {

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
            float x = Mathf.Cos(Angle) * Radius;
            float z = Mathf.Sin(Angle) * Radius;
            transform.position = new Vector3(Postion.x + x, fixedY, Postion.z + z);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnEnable()
    {
        Postion = transform.position;

        int RandomScale = Random.Range(1, 4);
        transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);
        fixedY = RandomScale / 2f;

        //랜덤 각도에서 시작
        Angle = Random.Range(0f, Mathf.PI * 2f);

        //반지름 위치 
        Radius = Random.Range(3f, 20f);

        //랜덤 회전 방향(1 or -1)
        rotateDirection = Random.value < 0.5f ? 1 : -1;

        //원을 따라 도는 코루틴 시작
        rotateCoroutine = StartCoroutine(GoPewPew());//굳이 변수 선언한건 값 초기화 때문
    }


    // 오브젝트가 꺼질 때 코루틴 정리
    void OnDisable()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
        rotateCoroutine = null;
    }

}

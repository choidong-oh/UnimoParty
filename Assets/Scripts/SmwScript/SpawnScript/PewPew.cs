using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

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

    [SerializeField] GameObject CrashPewPew;

    Collider myCollider;

    float MoveSpeedSave;

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    myCollider = GetComponent<Collider>();
    //    myCollider.enabled = false;
    //    // 1. Terrain 참조
    //    terrain = Terrain.activeTerrain;
    //    if (terrain == null)
    //    {
    //        Debug.LogWarning("트레인 없다 트레인쓰세요.");
    //    }
    //    else
    //    {
    //        Vector3 tPos = terrain.transform.position;
    //        Vector3 tSize = terrain.terrainData.size;

    //        float centerX = tPos.x + tSize.x * 0.5f;
    //        float centerZ = tPos.z + tSize.z * 0.5f;

    //        Position = new Vector3(centerX, 0, centerZ);//트레인기준 중심
    //    }

    //    //랜덤 몬스터 크기
    //    float RandomScale = Random.Range(1, 4) * 0.3f;
    //    transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);

    //    //랜덤 각도에서 시작
    //    Angle = Random.Range(0f, Mathf.PI * 2f);
    //    //랜덤반지름 위치 
    //    Radius = Random.Range(3f, 20f);
    //    //랜덤 회전 방향(1 or -1)
    //    rotateDirection = Random.value < 0.5f ? 1 : -1;
    //    rotateCoroutine = StartCoroutine(GoPewPew());//굳이 변수 선언한건 값 초기화 때문
    //}


    public override void OnDisable()
    {
        base.OnDisable();
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }

    }

    IEnumerator GoPewPew()
    {
        myCollider.enabled = true;
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
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashPewPew, hitPoint, rot);


            gameObject.SetActive(false);
        }

    }

    public override void Move(Vector3 direction)
    {
        photonView.RPC("MoveRPC", RpcTarget.All, direction);
    }

    [PunRPC]
    public void MoveRPC(Vector3 direction)
    {
        myCollider = GetComponent<Collider>();
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

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction);
    }

    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)
        {
            MoveSpeedSave = MoveSpeed;
            MoveSpeed = 0;

        }
        else if (isFreeze == false)
        {
            MoveSpeed = MoveSpeedSave;
        }
        else
        {
            Debug.Log("퓨퓨 프리즈 고장남");
        }
    }

}

//퓨퓨
using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PewPew : EnemyBase
{

    Vector3 Position;//	Terrain 중앙 좌표를 저장. 원형 궤도를 그릴 중심점으로 사용

    float Radius; // Position을 기준으로 이동할 궤도의 반지름
    float MoveSpeed = 10; //이동속도 
    float Angle; //현재 회전 각도로 쓸것 

    int rotateDirection;   //어디로갈지 시계 반시계
    float fixedY = 0f;// Terrain 기준으로 높이를 마춰주는 변수

    Terrain terrain; //Terrain 의 데이터랑 해당 Terrain의 높이를 받아옴

    [SerializeField] GameObject CrashPewPew; // 퓨퓨가 죽으면 생성해줄 파티클

    Collider myCollider;// 몬스터 충돌 관리 

    float MoveSpeedSave;// 프리즈 상태일때 현제 이동속도를 담아줄변수 

    PewPewSp Spawner; //퓨퓨 전용 스포너랑 연결해줄 것

    Animator animator; //퓨퓨 관련 애니메이션 

    [SerializeField] float FreezeTime = 3; //얼음 상태일때 해동되면 해동되는데 걸리는시간

    [SerializeField] GameObject IsFreeze; //퓨퓨 얼음상태를 보여줄 프리팹

    Vector3 newPos;//매 프레임 계산한 다음 이동할 위치 저장

    Vector3 moveDir;//newPos - 현재위치로 방향회전 시 사용

    float terrainY;//지형 표면 높이+몬스터 절반 높이+fixedY 계산값

    [SerializeField] float CenterNoSpawn = 5f;// 중심 근처 스폰 방지 반경

    public override void OnEnable()
    {
        base.OnEnable();//포툰과 같이 쓸때 필요한함수 

        animator = GetComponent<Animator>();// 자신의 애니메이터를 담아줌

        if (Spawner == null)//퓨퓨전용 스포너가 등록이 안되 있으면  
        {
            Spawner = FindObjectOfType<PewPewSp>();//Spawner에 PewPewSp 컴포너트가 있는걸 담아라 ObjectOfType는 오브젝트가 1개만있으면 써도됨 그외는 x
        }

        myCollider = GetComponent<Collider>();//myCollider에 나 자신 충돌체를 넣어줄꺼임 

        myCollider.enabled = false;//일단 충돌체를꺼라 

        terrain = Terrain.activeTerrain;//terrain 에 Terrain 을 정보를 담아줌

        if (terrain == null) //terrain 안에 값이 없으면
        {
            Debug.LogWarning("트레인 없다 트레인쓰세요.");
        }
        else
        {
            Vector3 tPos = terrain.transform.position;//Terrain 좌표 
            Vector3 tSize = terrain.terrainData.size;//Terrain 크기 

            float centerX = tPos.x + tSize.x * 0.5f;//이거는 Terrain 중심 좌표를 담아줄 변수 x축
            float centerZ = tPos.z + tSize.z * 0.5f;//이거는 Terrain 중심 좌표를 담아줄 변수 z축

            Position = new Vector3(centerX, 0, centerZ);//트레인기준 중심
        }

        float RandomScale = Random.Range(1, 4) * 0.5f;//이거는 퓨퓨 크기를 정해주는 변수 

        transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);// 퓨퓨의 크기를 적용시키는 곳

        Angle = Random.Range(0f, Mathf.PI * 2f);//
        Radius = Random.Range(CenterNoSpawn, 20f);//여기서 원의 크기가 다라짐 요약하면 반지름을 정해줌 가운데는 오브때문에 CenterNoSpawn로 금지지역을 정해줌
        rotateDirection = Random.value < 0.5f ? 1 : -1;//이거는 퓨퓨가 시계방향으로 돌껀지 반시계 방향으로 돌껀지 정해주는 곳 (추가정보 Random.value는 0.0 ~ 1.0을 랜덤으로 뽑고 1, -1을 정해준다 )
        StartCoroutine(GoPewPew());//GoPewPew() 코루틴을 실행해라 

    }


    public override void OnDisable()
    {
        base.OnDisable();
        myCollider.enabled = false;//충돌체를 꺼줘라 
        ImFreeze = false;// enemybase에 있는 나얼음상태인지 아닌지를 얼음상태 아니라고 초기화
    }




    IEnumerator GoPewPew()
    {
        float angularSpeed = MoveSpeed / Radius;//몬스터 이동속도 / 원의 반지름  해서 각속도를 구함 
        Angle -= angularSpeed * Time.deltaTime * rotateDirection;// 각속도 * 프레임 경과 시간 * 어느방향으로 돌껀지

        float x = Position.x + Mathf.Cos(Angle) * Radius;//cos(Angle)으로 단위 원 상의 방향 얻고, 반지름 곱해 실제 거리로 확대
        float z = Position.z + Mathf.Sin(Angle) * Radius;//sin(Angle)으로 단위 원 상의 방향 얻고, 반지름 곱해 실제 거리로 확대

        terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;//terrainY에 transform.position 자기위치에  terrain.SampleHeight(트레인에 높이)에 퓨퓨 자신의 크기의 절반만큼 + fixedY 더 한높이로 조정
        newPos = new Vector3(x, terrainY, z);// 최종 목적지 좌표 생성

        moveDir = (newPos - transform.position).normalized; //normalized: 단위 벡터. LookRotation에 사용

        if (moveDir.sqrMagnitude > 0.001f) //너무심하게 작은 원 막기위한 코드
        {
            transform.rotation = Quaternion.LookRotation(moveDir);//퓨퓨의 회전값을 회전하는 방향으로 즉 앞을 보게 함 
        }
        transform.position = newPos;// 회전 할 위치로 즉시 위치 이동


        animator.speed = 0f;//애니메이션 잠시 멈춰줌 (스폰 때문에)
        yield return new WaitForSeconds(1.5f);// 스폰때문에 1.5초기달리고 
        animator.speed = 1f;//애니메이션 움직이고 

        myCollider.enabled = true;//충돌체를킴 

        while (true)
        {
            angularSpeed = MoveSpeed / Radius; //원 둘레를 도는 속도
            Angle -= angularSpeed * Time.deltaTime * rotateDirection;//각도를 회전 방향에 따라 바꿔줌

            //위치 계산해서 이동
            x = Position.x + Mathf.Cos(Angle) * Radius;//cos(Angle)으로 단위 원 상의 방향 얻고, 반지름 곱해 실제 거리로 확대
            z = Position.z + Mathf.Sin(Angle) * Radius;//Sin(Angle)으로 단위 원 상의 방향 얻고, 반지름 곱해 실제 거리로 확대

            terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;// 위에 적어 놓았으니 요약: 지형에 마춰 몬스터의 높이 중간만큼 올림 + fixedY
            newPos = new Vector3(x, terrainY, z);// 최종 목적지 좌표 생성

            moveDir = (newPos - transform.position).normalized;//normalized: 단위 벡터. LookRotation에 사용

            if (moveDir.sqrMagnitude > 0.001f) //너무심하게 작은 원 막기위한 코드
            {
                transform.rotation = Quaternion.LookRotation(moveDir);//퓨퓨의 회전값을 회전하는 방향으로 즉 앞을 보게 함 
            }

            transform.position = newPos;// 회전 할 위치로 즉시 위치 이동

            yield return new WaitForFixedUpdate();//0.02초 마다 반복 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;//객체가 나의 것이 아니면 함수종료 

        if (other.CompareTag("Player")) //other 의 태그가 Player 일 경우 
        {
            if (ImFreeze == true)//내가만약 얼어 있으며
            {
                ImFreeze = false;//얼어있는걸 해제하라
                StartCoroutine(FreezeCor());//얼음 해제 코루틴 실행
            }
            else if (ImFreeze == false)// 얼음상태가 아닐경우 
            {
                damage = 1;// 몬스터의 데미지는 1이다
                Manager.Instance.observer.HitPlayer(damage);// 싱글톤으로 저장 되어 있는 HitPlayer 함수를 실행하여 플레이어에게 데미지를 준다.


                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 위에서 구한 방향(normal)을 앞(direction)으로 삼아 회전(Quaternion) 생성
                Instantiate(CrashPewPew, hitPoint, rot);// 위에 3개를 적용해서 퓨퓨 사망 파티클 생성

                if (!PhotonNetwork.IsMasterClient) return;//방장아니면 함수를 종료해라 
                Spawner.SpawnOne();// Spawner안에 있는 SpawnOne() 실행 SpawnOne()은 퓨퓨가 죽으면 다시생성하게 할꺼임
                if (PhotonNetwork.IsMasterClient)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }
            }
        }

        else if (other.CompareTag("Monster"))//태그가 몬스터일경우 
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>();//몬스터끼리 충돌한거라 other 엔 EnemyBase 가 있음 otherEnemy 담아줌

            if (otherEnemy == null)//이코드는 전체적으로 otherEnemy 안에 값이 없을경우 없다고 알림
            {
                Debug.Log("몬스터 EnemyBase 가 없음");
                return;
            }

            if (ImFreeze == false && otherEnemy.ImFreeze == true)//나는 빙결 상태가 아닌데 상대는 빙결상태 일경우 
            {

                otherEnemy.ImFreeze = false;//상대방에 빙결을 풀어준다 
                otherEnemy.Move();// 여기에 FreezeCor() 에 들어있는 빙결해제를 담겨있어서  실행시킴 

                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 위에서 구한 방향(normal)을 앞(direction)으로 삼아 회전(Quaternion) 생성
                Instantiate(CrashPewPew, hitPoint, rot);// 위에 3개를 적용해서 퓨퓨 사망 파티클 생성

                if (!PhotonNetwork.IsMasterClient) return;//방장만 실행하게 
                Spawner.SpawnOne();// Spawner안에 있는 SpawnOne() 실행 SpawnOne()은 퓨퓨가 죽으면 다시생성하게 할꺼임
                if (PhotonNetwork.IsMasterClient)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }

            }
        }


        else if (other.CompareTag("Aube"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
            Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
            Quaternion rot = Quaternion.LookRotation(normal);// 위에서 구한 방향(normal)을 앞(direction)으로 삼아 회전(Quaternion) 생성
            Instantiate(CrashPewPew, hitPoint, rot);// 위에 3개를 적용해서 퓨퓨 사망 파티클 생성

            if (!PhotonNetwork.IsMasterClient) return;//방장만 실행하게 
            Spawner.SpawnOne();// Spawner안에 있는 SpawnOne() 실행 SpawnOne()은 퓨퓨가 죽으면 다시생성하게 할꺼임
            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }
        }

    }



    public override void Move()
    {
        StartCoroutine(FreezeCor());//해동 시켜주는 코루틴실행 
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction, isFreeze);// 모든 대상에게 FreezeRPC 함수를 쏴줌
    }

    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)// 처음 얼음 상태를 처리해줄 함수 
    {
        if (isFreeze == true)//얼음!! 걸리면 
        {
            ImFreeze = isFreeze;// ImFreeze는 isFreeze가 true 이니깐 true를 담아줌 
            IsFreeze.SetActive(true);//이거는 몬스터가 얼음이라는걸 외형적으로 보여줌 (얼음 활성화)
            MoveSpeedSave = MoveSpeed;//얼음 상태 에서는 이동도 금지해야하니깐 나중에 다시 속도가 돌아올때 기존 이동속도를 저장함
            MoveSpeed = 0;// 이제 이동을 금지시킴 
            animator.speed = 0f;//얼음상태이니 애니메이션도 잠시 멈춤 
        }
        else if (isFreeze == false)
        {
            Debug.Log(isFreeze + " 프리즈 해제");// 이거 넘어 오긴하는건가
            ImFreeze = isFreeze;//이거 넘어오면 false임
            StartCoroutine(FreezeCor());//행동 시켜줌
        }
        else
        {
            Debug.LogWarning("퓨퓨 프리즈 고장남");
        }
    }
    IEnumerator FreezeCor()
    {

        yield return new WaitForSeconds(FreezeTime);//해동시키는데 기달리는시간 
        MoveSpeed = MoveSpeedSave;//이동속도를 원레속도로 바꿔줄꺼임
        animator.speed = 1f;//애니메이션 다시 움직이게 함
        IsFreeze.SetActive(false);//얼음 프리팹 비활성화 
    }


}

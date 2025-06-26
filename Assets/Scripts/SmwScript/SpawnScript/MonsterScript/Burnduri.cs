//번드리
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnduri : EnemyBase
{

    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    Vector3 Target; //현재 추적 목표 위치를 저장할 변수
    Vector3 myPos; //자신의 현재 위치를 저장할 변수

    [Header("이동 설정")]
    [SerializeField] float MoveSpeed = 1f;// 기본 이동속도
    [SerializeField] float chargeSpeed = 10f; // 차징할때 이동속도 
    [SerializeField] float chargeDistance = 3f;// 해당 범위안에 들어오면 돌진으로 바뀌는 거리

    [Header("거리 조건")]
    [SerializeField] float triggerDistance = 10f;// 돌진 할때 이동하는거리 
    [SerializeField] float fixedY = 0f;//추가적인 높이 조정 필요할때 


    bool isCharging = false; //내가 차징 상태인지 아닌지 체크 


    Terrain terrain; //Terrain 의 정보를 받아올 Terrain변수 

    private Coroutine updateRoutine;// 코루틴을 담아줄 변수 (코루틴을 멈춰야 되는 상황이 있기 때문에)


    Transform nearestPlayer; //플레이어중에서 제일 가까운애를 담을 Transform변수  

    Animator animator; //자기자신을 담을 예정인 애니메이터 변수
    Collider myCollider;// 자기자신 충돌체를 관리한 충돌체 변수 

    [SerializeField] GameObject CrashBurnduri; //번드리가 죽으면 번드리전용 죽음 파티클

    [SerializeField] float FreezeTime = 3; // 번드리가 해동되는데 걸리는 시간 

    [SerializeField] GameObject IsFreeze;// 번드리가 얼었을때 킬 얼음 프리팹

    string AppearAni = "anim_01_MON001_Bduri_Appearance"; //번드리 처음 스폰 애나메이션 이름 
    string state3 = "anim_03_MON001_Bduri_Encounter"; //차징을 하기전 차징준비 에니메이션 이름 
    string state5 = "anim_01_MON001_Bduri_Disappearance";// 번드리가 사라질때 쓸 애니메이션 이름



    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;//객체가 나의 것이 아니면 함수종료 
        if (other.CompareTag("Player"))//충돌체가 닿은게 플레이어라는 테크를 달고있으면  
        {
            if (ImFreeze == true)//내가만약 얼음 상태가 켜져있으면 
            {
                ImFreeze = false;// 나얼음 상태 아니라고 할꺼임 
                StartCoroutine(FreezeCor());//나자신을 해동함 
            }
            else if (ImFreeze == false)// 내가 빙결이 아니면 
            {
                damage = 1;//데미지는 일단 1로함 
                Manager.Instance.observer.HitPlayer(damage);// 싱글톤으로 저장 되어 있는 HitPlayer 함수를 실행하여 플레이어에게 데미지를 준다.

                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
                Instantiate(CrashBurnduri, hitPoint, rot);//번드리전용 파티클을 생성함 

                if (PhotonNetwork.IsMasterClient)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);//번드리를 풀로 반환
                }

            }
        }

        else if (other.CompareTag("Monster"))//몬스터 테크가 달린 오브젝트 일경우 
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
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
                Instantiate(CrashBurnduri, hitPoint, rot);// 번드리 사망 파티클 생성 
                if (PhotonNetwork.IsMasterClient)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);//나자신을 PoolManager에 반환
                }

            }
        }

        else if (other.CompareTag("Aube"))//오브에 충돌될경우 
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
            Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
            Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
            Instantiate(CrashBurnduri, hitPoint, rot);// 번드리 사망 파티클 생성 
            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.DespawnNetworked(gameObject);//나자신을 PoolManager에 반환
            }
        }
    }


    public override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();//자신의 애니메이션을 넣는다 animator에
        myCollider = GetComponent<Collider>();// 나자신의 충돌체를 넣음 myCollider에

        myCollider.enabled = false;//처음에 스폰 때문에 잠시 충돌을 꺼준다

        terrain = Terrain.activeTerrain; //Terrain의 정보를 담는다 
        isCharging = false;//차징 상태가 아니므로 초기엔 false로해준다 

        StartCoroutine(GoBurnduri());// GoBurnduri() 코루틴을 실행 한다 
    }

    void FindPlayer()// 해당 함수는 플레이어를 찾는 함수임 
    {
        if (players.Count == 0)// 플레이어가 없으면 
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");//하이러키에 플레이어 테그 달린애들을 찾는다 
            foreach (var obj in objs)
            {
                players.Add(obj.transform);//리스트에 플레이어 추가
            }
        }
    }

    IEnumerator GoBurnduri()
    {

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));// AppearAni 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))// 지금 플레이 중인 애니메이션이 AppearAni 라는 이름일 동안 계속 반복하라
            yield return null; // 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        myCollider.enabled = true;// 번드리 충돌체를 다시 켜줌 스폰장면이 끝났기 때문에 

        updateRoutine = StartCoroutine(UpdateDistance()); // UpdateDistance() 코루틴을 실행함 그냥 스타트 코루틴이랑 다른점은 이거는 중간에 스탑 코루틴으로 멈출수 있어서   
        StartCoroutine(MoveRoutine());//  MoveRoutine() 을 실행해라 
    }


    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();//모든 코루틴을 종료 할때
        myCollider.enabled = false;//충돌체를 꺼준다
    }


    IEnumerator UpdateDistance()//가까운 플레이어 거리 계산이랑 가까운 플레이어 방향을 쳐다보는걸 해주는 코루틴 
    {
        while (true)
        {
            if (players != null && players.Count == 0)//플레이어 리스트틑 존재하나 플레이어가 없으면 
            {
                FindPlayer();// 플래이어 찾는함수 실행
                yield return null;
            }

            for (int i = players.Count - 1; i >= 0; i--)//플레이어 리스트 안에 애들검사 
            {
                Transform p = players[i];// 일단 플레이어 자표

                if (p == null || !p.gameObject.activeInHierarchy)//플레이어 좌표가 없다고뜨거나 하이렄에 없으면 
                {
                    players.RemoveAt(i);// 그 플레이어 좌표는 제거 한다 
                }
            }

            myPos = transform.position;
            float minDistance = Mathf.Infinity;// 일단 제일 큰값으로함 


            foreach (var player in players)//플레이어들 수만큼반복
            {


                float Distance = Vector3.Distance(myPos, player.position);//플레이어와 번드리르 거리를 Distance에 담음

                if (Distance < minDistance)//이전까지 찾았던 최소 거리보다 작으면 갱신
                {
                    minDistance = Distance;
                    nearestPlayer = player;
                }
            }

            if (nearestPlayer != null)//nearestPlayer 가 없는게 아니면
            {
                Target = nearestPlayer.position;//Target에 nearestPlayer.position 값을 담는다 제일 가까운 애 이기때문

                transform.LookAt(Target);// 제일 가까운애를 쳐다보게 하는 LookAt 함수 
            }


            yield return new WaitForSeconds(0.5f);// 너무 자주반복하면 렉걸리니 0.5초마다 해라
        }
    }


    IEnumerator MoveRoutine()//실질적으로 앞으로 가는코루틴
    {
        while (true)
        {
            if (!isCharging)//차징상태가 아니라면 
            {
                float CheckNear = Vector3.Distance(myPos, Target); // 번드링와 가까운 플레이어의 거리를 CheckNear 에넣음

                if (CheckNear < chargeDistance)// 돌진 거리 이내에 들어오면 chargeDistance가 더크면 거리에 도달한거라서 
                {

                    if (updateRoutine != null)//혹시없는거 체크 
                    {
                        StopCoroutine(updateRoutine);//방향 잡아주는 코루틴 멈춤 
                    }
                    isCharging = true;//앞으로 더가면 안되니깐 앞의로 이동을 막음


                    StartCoroutine(ChargeRoutine());// 일직선으로 차징하는 코루틴 실행 

                    yield break;//지금 앞으로가는 코루틴도 종료 
                }
                else
                {
                    myPos = transform.position;// 다시한번 자기위치 저장
                    float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY; //지금 번드리가 있는 위치에 트레인 높이 값에 번드리 y크기 절반만큼 높이로 지정 + fixedY
                    transform.position = new Vector3(myPos.x, terrainY, myPos.z);// 여기에서 높이를 적용
                    transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;// 앞으로 이동속도만큼 이동 (Time.fixedDeltaTime는 0.02초마다 갱신한다 )
                }
            }
            yield return new WaitForFixedUpdate();//0.05초마다 반복
        }
    }

    IEnumerator ChargeRoutine()//이제 차징을 하는 코루틴 
    {
        animator.SetTrigger("ChargeStart");//애니메이터에 ChargeStart 라는 트리거를 실행

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state3));// state3 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state3))// 지금 플레이 중인 애니메이션이 state3 라는 이름일 동안 계속 반복하라
            yield return null;// 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        Vector3 startPos = transform.position;// 차징 하기전 차징 시작위치를 저장 

        Quaternion Rot = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z); // Y, Z 회전값만 유지하고 X 회전은 0으로 고정 왜냐면 앞으로만 가기때문

        transform.rotation = Rot;// 회전값을 적용


        //triggerDistance이상이되면 빠져나가는데 triggerDistance는 차징거리임 Vector3.Distance(transform.position, startPos)이거는 처음시작위치와의 거리
        while (Vector3.Distance(transform.position, startPos) < triggerDistance)
        {
            myPos = transform.position;
            float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY; //지금 번드리가 있는 위치에 트레인 높이 값에 번드리 y크기 절반만큼 높이로 지정 + fixedY
            transform.position = new Vector3(myPos.x, terrainY, myPos.z);//그 높이를 적용 
            transform.position += transform.forward * chargeSpeed * Time.fixedDeltaTime;// 앞으로가는코드에서 chargeSpeed 으로 바꿔서 적용함 
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(DieBurnduri());//다끝나면 번드리는 죽어야함 죽는 코루틴임
    }

    IEnumerator DieBurnduri()//번드리 차징다하고 죽을때 쓰는 코루틴 
    {
        myCollider.enabled = false;//충돌체를 꺼준다 
        animator.SetTrigger("disappear");//애니메이터에 사라질때쓰는 트리거를 발동시킨다
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state5));// state5 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state5))// 지금 플레이 중인 애니메이션이 state5 라는 이름일 동안 계속 반복하라
            yield return null;// 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        if (PhotonNetwork.IsMasterClient)//마스터 클라이언트만 멀티일때 다른애들도 할수 있긴때문 
        {
            PoolManager.Instance.DespawnNetworked(gameObject); //PoolManager로 반환 
        }
    }


    public override void Move()
    {
        StartCoroutine(FreezeCor());//해동할때 쓰는 코루틴
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction, isFreeze);
    }

    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)//얼음 폭탄에 맞으면 
        {
            StopAllCoroutines();//모든코루틴을 멈춤 
            ImFreeze = isFreeze;//빙결상태 저장 
            IsFreeze.SetActive(true);//얼음 프리팹 키기
            animator.speed = 0f;//애니메이션 재생을 멈춤 
        }
        else if (isFreeze == false)//빙결 상태가 풀리면 
        {
            ImFreeze = isFreeze;//빙결상태 저장 
            StartCoroutine(FreezeCor());//해동 코루틴발동
        }
        else
        {
            Debug.Log("번드리 프리즈 고장남");
        }
    }

    IEnumerator FreezeCor()// 번드리 해동하는 코루틴
    {
        yield return new WaitForSeconds(FreezeTime);//빙결상태 풀리기까지 걸리는 시간 
        animator.speed = 1f;//애니메이션 다시재생 
        StartCoroutine(GoBurnduri());//다시 번드리 행동을 켜줄꺼임 
        IsFreeze.SetActive(false);//얼음 프리팹을 꺼줌 

    }


}

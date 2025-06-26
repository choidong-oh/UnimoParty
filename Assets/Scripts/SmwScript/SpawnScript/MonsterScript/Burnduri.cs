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
                Quaternion rot = Quaternion.LookRotation(normal);// 위에서 구한 방향(normal)을 앞(direction)으로 삼아 회전(Quaternion) 생성
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
                Quaternion rot = Quaternion.LookRotation(normal);// 위에서 구한 방향(normal)을 앞(direction)으로 삼아 회전(Quaternion) 생성
                GameObject inst = Instantiate(CrashBurnduri, hitPoint, rot);// 


                if (PhotonNetwork.IsMasterClient)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }

            }
        }

        else if (other.CompareTag("Aube"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Vector3 normal = (hitPoint - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(normal);

            GameObject inst = Instantiate(CrashBurnduri, hitPoint, rot);


            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }

        }
    }


    public override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();

        myCollider.enabled = false;

        terrain = Terrain.activeTerrain;
        isCharging = false;

        StartCoroutine(GoBurnduri());
    }

    void FindPlayer()
    {
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objs)
            {
                players.Add(obj.transform);
            }
        }
    }

    IEnumerator GoBurnduri()
    {

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))
            yield return null;

        myCollider.enabled = true;

        updateRoutine = StartCoroutine(UpdateDistance());
        StartCoroutine(MoveRoutine());
        yield return null;
    }


    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        myCollider.enabled = false;
    }


    IEnumerator UpdateDistance()
    {
        while (true)
        {
            if (players != null && players.Count == 0)
            {
                FindPlayer();
                yield return null;
            }

            for (int i = players.Count - 1; i >= 0; i--)
            {
                Transform p = players[i];

                if (p == null || !p.gameObject.activeInHierarchy)
                {
                    players.RemoveAt(i);
                }
            }

            myPos = transform.position;
            float minDistance = Mathf.Infinity;// 일단 제일 큰값으로함 


            foreach (var player in players)
            {

                // 거리계산
                float Distance = Vector3.Distance(myPos, player.position);

                //이전까지 찾았던 최소 거리보다 작으면 갱신
                if (Distance < minDistance)
                {
                    minDistance = Distance;
                    nearestPlayer = player;
                }
            }

            if (nearestPlayer != null)
            {
                Target = nearestPlayer.position;

                transform.LookAt(Target);
            }


            yield return new WaitForSeconds(0.5f);
        }
    }


    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (!isCharging)
            {
                float CheckNear = Vector3.Distance(myPos, Target);

                if (CheckNear < chargeDistance)
                {

                    if (updateRoutine != null)
                    {
                        StopCoroutine(updateRoutine);
                    }
                    isCharging = true;


                    StartCoroutine(ChargeRoutine());

                    yield break;
                }
                else
                {
                    myPos = transform.position;
                    float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;
                    transform.position = new Vector3(myPos.x, terrainY, myPos.z);
                    transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ChargeRoutine()
    {
        animator.SetTrigger("ChargeStart");

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state3));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state3))
            yield return null;

        Vector3 startPos = transform.position;


        float yAngle = transform.eulerAngles.y;
        float zAngle = transform.eulerAngles.z;

        Quaternion Rot = Quaternion.Euler(0f, yAngle, zAngle);

        transform.rotation = Rot;

        while (Vector3.Distance(transform.position, startPos) < triggerDistance)
        {
            myPos = transform.position;
            float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;
            transform.position = new Vector3(myPos.x, terrainY, myPos.z);
            transform.position += transform.forward * chargeSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        StartCoroutine(DieBurnduri());
    }

    IEnumerator DieBurnduri()
    {
        myCollider.enabled = false;
        animator.SetTrigger("disappear");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state5));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state5))
            yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            PoolManager.Instance.DespawnNetworked(gameObject);
        }

    }


    public override void Move()
    {
        StartCoroutine(FreezeCor());
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction, isFreeze);
    }

    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)
        {
            StopAllCoroutines();
            ImFreeze = isFreeze;
            IsFreeze.SetActive(true);
            animator.speed = 0f;
        }
        else if (isFreeze == false)
        {
            Debug.Log(isFreeze + " 프리즈 해제");// 이거 넘어 오긴하는건가
            ImFreeze = isFreeze;
            StartCoroutine(FreezeCor());
        }
        else
        {
            Debug.Log("번드리 프리즈 고장남");
        }
    }

    IEnumerator FreezeCor()
    {
        yield return new WaitForSeconds(FreezeTime);
        animator.speed = 1f;
        StartCoroutine(GoBurnduri());
        IsFreeze.SetActive(false);

    }


}

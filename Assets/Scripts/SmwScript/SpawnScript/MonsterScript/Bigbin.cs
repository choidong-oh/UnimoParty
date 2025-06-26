//빅빈
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bigbin : EnemyBase
{

    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    [Header("이동 설정")]
    float MoveSpeed = 5f;//현제속도
    float FirstSpeed; //처음속도를 저장할 변수


    [Header("거리 조건")]
    [SerializeField] float fixedY = 0f;//빅빈 높이를 조정할 변수 

    Terrain terrain; //트레인을 담을 변수 

    Collider myCollider;//자기자신의 충동체를담을 변수

    [SerializeField] GameObject CrashBigbin;// 빅빈 죽을때 쓸파티클

    [SerializeField] GameObject JumpParticles;//점프하고 땅에 닿을때마다 생성 파티클

    [SerializeField] GameObject JumpExplode;//점프 종료되면 빅빈이 사라지는 파티클

    Transform nearestPlayer; // 가장 가까운 플레이어 Transform을 저장할 변수

    Vector3 myPos;// 빅빈의 현재 위치를 저장할 변수
    Vector3 Target;// 최종 가까운 플레이어 위치를 저장할 변수

    Animator animator;//빅빈의 애니메이터를 담을 변수

    [SerializeField] float FreezeTime = 3; //해동할때 걸리는시간 

    //애니메이션 스테이트 이름들
    string AppearAni = "anim_MON004_appear";
    string state2 = "anim_MON004_readytojump";
    string state3 = "anim_MON004_jump01";
    string state4 = "anim_MON004_jump02";
    string state5 = "anim_MON004_jump03";

    [SerializeField] GameObject IsFreeze;//얼음 프리팹

    float MoveSpeedSave;//현제 이동속도를담을 변수

    public override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();// animator에 애니메이터를 담음
        myCollider = GetComponent<Collider>();//myCollider에 자신의 충돌체를넣음
        terrain = Terrain.activeTerrain;//terrain에 트레인정보를 넣음
        FirstSpeed = MoveSpeed / 2;//현제 이동속도에 0.5만큼 증가시킬꺼기때문에
        StartCoroutine(GoBigBin());//GoBigBin() 코루틴실행 
    }


    public override void OnDisable()
    {
        base.OnDisable();
        MoveSpeed = FirstSpeed * 2;//값을 다시 초기화해줌
        myCollider.enabled = false;//충돌체 다시꺼줌
    }


    IEnumerator GoBigBin()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));// AppearAni 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))// 지금 플레이 중인 애니메이션이 AppearAni 라는 이름일 동안 계속 반복하라
            yield return null;// 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        myCollider.enabled = true;//충돌체를 다시 키기 

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state2));//state2 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state2))// 지금 플레이 중인 애니메이션이 state2 라는 이름일 동안 계속 반복하라
            yield return null;// 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        JumpParticles.SetActive(true);//점프 파티클 활성화 
        MoveSpeed += FirstSpeed;//속도 증가


        StartCoroutine(MoveRoutine());//앞으로 가는 코루틴 시작
        StartCoroutine(UpdateDistance());//방향을 잡아주는 코루틴 시작

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state3));//state3 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state3))// 지금 플레이 중인 애니메이션이 state3 라는 이름일 동안 계속 반복하라
            yield return null;// 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        JumpParticles.SetActive(true);//점프 파티클 활성화 
        MoveSpeed += FirstSpeed;//속도 증가


        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state4));//state4 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state4))// 지금 플레이 중인 애니메이션이 state4 라는 이름일 동안 계속 반복하라
            yield return null;// 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        JumpParticles.SetActive(true);//점프 파티클 활성화 
        MoveSpeed += FirstSpeed;//속도 증가

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state5));//state5 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)// 지금 플레이 중인 애니메이션이 완전히 재생될때까지 계속 반복하라
            yield return null;// 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        Instantiate(JumpExplode, transform.position, Quaternion.identity);//점프다하고 마지막에 빅빈이 사라지는 파티클생성
        StopAllCoroutines();//모든코루틴 멈추게하기

        if (PhotonNetwork.IsMasterClient)
        {
            PoolManager.Instance.DespawnNetworked(gameObject);//PoolManager에 자신을 반환
        }

    }



    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.CompareTag("Player"))
        {
            if (ImFreeze == true)
            {
                ImFreeze = false;
                StartCoroutine(FreezeCor());
            }
            else if (ImFreeze == false)
            {
                damage = 1;
                Manager.Instance.observer.HitPlayer(damage);

                Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게
                Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
                Quaternion rot = Quaternion.LookRotation(normal);// 방향계산
                Instantiate(CrashBigbin, hitPoint, rot);

                if (PhotonNetwork.IsMasterClient)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }

            }
        }

        else if (other.CompareTag("Monster"))
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>();

            if (otherEnemy == null)
            {
                Debug.Log("몬스터 EnemyBase 가 없음");
                return;
            }

            if (ImFreeze == false && otherEnemy.ImFreeze == true)
            {

                otherEnemy.ImFreeze = false;
                otherEnemy.Move();

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 normal = (hitPoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(normal);
                Instantiate(CrashBigbin, hitPoint, rot);
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

            Instantiate(CrashBigbin, hitPoint, rot);

            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }

        }

    }

    IEnumerator MoveRoutine()
    {

        while (true)
        {
            myPos = transform.position;
            float terrainY = terrain.SampleHeight(transform.position) + fixedY;
            transform.position = new Vector3(myPos.x, terrainY, myPos.z);

            transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator UpdateDistance()
    {
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objs)
            {
                players.Add(obj.transform);
            }
        }
        while (true)
        {
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
                float Distance = Vector3.Distance(myPos, player.position);


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
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }


            yield return new WaitForSeconds(0.5f);
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
            ImFreeze = isFreeze;
            IsFreeze.SetActive(true);
            MoveSpeedSave = MoveSpeed;
            MoveSpeed = 0;
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
            Debug.Log("빅빈 프리즈 고장남");
        }
    }

    IEnumerator FreezeCor()
    {
        yield return new WaitForSeconds(FreezeTime);
        MoveSpeed = MoveSpeedSave;
        animator.speed = 1f;
        IsFreeze.SetActive(false);
    }

}

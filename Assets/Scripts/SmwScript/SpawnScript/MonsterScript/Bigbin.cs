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
        if (!photonView.IsMine) return;//객체가 나의 것이 아니면 함수종료 
        if (other.CompareTag("Player"))//해당객체가 플레이어일경우 
        {
            if (ImFreeze == true)//내가 프리즈상태이면
            {
                ImFreeze = false;//프리즈상태 제거 
                StartCoroutine(FreezeCor());//해동 코루틴 발동 
            }
            else if (ImFreeze == false)//내가 프리즈 상태가 아니면 
            {
                damage = 1;//임의로 데미지를 1로 줌
                Manager.Instance.observer.HitPlayer(damage);//플레이어에게 데미지를준거

                Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향
                Instantiate(CrashBigbin, hitPoint, rot);//빅빈이 터지는 파티클 생성 

                if (PhotonNetwork.IsMasterClient)//마스터클라이언트만
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);//PoolManager에 반환할꺼임
                }

            }
        }

        else if (other.CompareTag("Monster"))//테그가 몬스터라면 
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>();//Monster 종류들은 EnemyBase스크립트가 있으므로 연결해줌

            if (otherEnemy == null)//연결안됬으면 
            {
                Debug.Log("몬스터 EnemyBase 가 없음");
                return;
            }

            if (ImFreeze == false && otherEnemy.ImFreeze == true)//나는 프리즈가 아닌데 상대 몬스터가 프리즈일경우 
            {

                otherEnemy.ImFreeze = false;//상대방 프리즈는 풀어준다 
                otherEnemy.Move();//상대 해동상태로 만들어준다 

                Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향
                Instantiate(CrashBigbin, hitPoint, rot);//빅빈이 터지는 파티클 생성 
                if (PhotonNetwork.IsMasterClient)//마스터클라이언트만
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }
            }
        }


        else if (other.CompareTag("Aube"))//충돌된게 오브이면 
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게
            Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
            Quaternion rot = Quaternion.LookRotation(normal);// 회전방향
            Instantiate(CrashBigbin, hitPoint, rot);//빅빈이 터지는 파티클 생성 

            if (PhotonNetwork.IsMasterClient)//마스터클라이언트만
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }

        }

    }

    IEnumerator MoveRoutine()//앞으로만 움직이는 코루틴
    {

        while (true)
        {
            myPos = transform.position;//높이 관련 필요한벡터
            float terrainY = terrain.SampleHeight(transform.position) + fixedY;//적용할 높이
            transform.position = new Vector3(myPos.x, terrainY, myPos.z);//높이적용

            transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;//앞으로만 이동

            yield return new WaitForFixedUpdate();//주기적으로
        }
    }

    IEnumerator UpdateDistance()
    {
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");//Player 라는테그가 애들 전부 찾기
            foreach (var obj in objs)
            {
                players.Add(obj.transform);//추격할때 쓸 리스트에 추가해줌
            }
        }
        while (true)
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                Transform p = players[i];

                if (p == null || !p.gameObject.activeInHierarchy)//만약 플레이어리스트에 해당플레이어가 없거나 하이러키창에도 없으면 
                {
                    players.RemoveAt(i);//리스트에서 지워라
                }
            }

            myPos = transform.position;//혹시모를 초기화 
            float minDistance = Mathf.Infinity;// 일단 제일 큰값으로함 

            foreach (var player in players)
            {
                float Distance = Vector3.Distance(myPos, player.position);//플레이어 거리를 빋아옴


                if (Distance < minDistance)//기존꺼보다 가까우면 바꿔주는것 
                {
                    minDistance = Distance;
                    nearestPlayer = player;// 이제 추격할 플레이어 nearestPlayer에 담아줌
                }
            }

            if (nearestPlayer != null)
            {
                Target = nearestPlayer.position;//타겟은 제일가까운에 이기때문에 Target에 담아줌

                transform.LookAt(Target);//타켓을 쳐다봄
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);// 한번쳐다보는데 y축을제외한 다른축이 뒤집히지않게 
            }


            yield return new WaitForSeconds(0.5f);//0.5마다 방향 찾음
        }
    }


    public override void Move()//다른대에서 빅빈이 해동되는걸 해줄 함수 
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

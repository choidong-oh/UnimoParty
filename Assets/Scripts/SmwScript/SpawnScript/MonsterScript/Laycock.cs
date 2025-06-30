//레이콕
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laycock : EnemyBase
{

    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    Vector3 myPos;// 자신의 현제 위치 
    Vector3 Target;//가까운 플레이어 위치 

    Transform nearestPlayer;//제일가까운 플레이어 Transform 받기

    [SerializeField] ParticleSystem ChargeParticles;//레이저 발사전 충전 파티클 
    [SerializeField] ParticleSystem ShootParticles;//레이저 발사되는 동안 나오는 파티클(빔)
    [SerializeField] GameObject DieParticles;//레이콕 죽는 파티클

    Terrain terrain;//땅지형 값을 받을변수

    float LazerLoopTime = 3;//쏘기직전에 해당 초간 기달리고 쏨

    Animator animator;//자신 애니메이터 받는변수

    Collider myCollider;//자기충돌체 받는변수
    string AppearAni = "anim_MON006_Appear";// 애메이션 스테이트 이름

    [SerializeField] float FreezeTime = 3;//해동 시간

    [SerializeField] GameObject IsFreeze;//얼음 상태 프리팹

    private Coroutine lazerCoroutine;//레이저 코루틴 처리를 위해 코루틴변수로 담아줌

    LaycockSP laycockSP;//레이콕 스포너랑연결 (특정상황에서만 레이콕잉 생성)

    private void OnTriggerEnter(Collider other)//충돌체 처리
    {
        if (!photonView.IsMine) return;//객체가 나의 것이 아니면 함수종료 
        if (other.CompareTag("Player"))//테그가 플레이어 일경우 
        {
            if (ImFreeze == true)// 내가 얼음상태이면
            {
                ImFreeze = false;//얼음상태 저장
                StartCoroutine(FreezeCor());// 해동코루틴
            }
            else if (ImFreeze == false)
            {
                damage = 1;//데미지 1
                Manager.Instance.observer.HitPlayer(damage);//플레이어게 데미지를 줌

                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
                Instantiate(DieParticles, hitPoint, rot);//사망 파티클

                laycockSP.DisCountLaycock();//숫자가 줄었다는걸 알려줌


                if (PhotonNetwork.IsMasterClient)//마스터 클라이언트만
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }

            }
        }

        else if (other.CompareTag("Monster"))//테그 몬스터 충돌
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
                Instantiate(DieParticles, hitPoint, rot);// 번드리 사망 파티클 생성 
                if (PhotonNetwork.IsMasterClient)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);//나자신을 PoolManager에 반환
                }

            }
        }

        else if (other.CompareTag("Aube"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
            Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
            Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
            Instantiate(DieParticles, hitPoint, rot);// 번드리 사망 파티클 생성 
            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.DespawnNetworked(gameObject);//나자신을 PoolManager에 반환
            }

        }

    }


    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Distance());//행동시작
    }


    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();//모든코루틴 정지
        myCollider.enabled = false;//충돌체 끄기 
    }

    IEnumerator Distance()
    {
        animator = GetComponent<Animator>();//자신 애니메이션 담기 
        myCollider = GetComponent<Collider>();//자신 충돌체 담기
        terrain = Terrain.activeTerrain;//지형정보 담기 
        laycockSP = FindObjectOfType<LaycockSP>();//스포너 정보 담기 (연결)

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));// AppearAni 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))// 지금 플레이 중인 애니메이션이 AppearAni 라는 이름일 동안 계속 반복하라
            yield return null; // 한 프레임만 쉬었다가 다음 프레임에 다시 이어서 실행하게함

        myCollider.enabled = true;//충돌체 키기 

        float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f;//높이값 담기 
        transform.position = new Vector3(transform.position.x, terrainY, transform.position.z);//높이값 적용
        if (players.Count == 0)//플레이어가 없으면 
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");//플레이어 테크달린애들 전부 찾기 
            foreach (var obj in objs)//플레이어 숫자만큼
            {
                players.Add(obj.transform);//리스트에 담기 
            }
        }
        myPos = transform.position;// 자신위치를 넣어줌
        float minDistance = Mathf.Infinity;// 일단 제일 큰값으로함 

        foreach (var player in players)
        {
            float Distance = Vector3.Distance(myPos, player.position);//플레이어와의 거리계산 


            if (Distance < minDistance)//누가 제일 가까운지 비교
            {
                minDistance = Distance;
                nearestPlayer = player;
            }
        }

        if (nearestPlayer != null)
        {
            Target = nearestPlayer.position;//타겟을 제일 가까운애로함 

            transform.LookAt(Target);//가까운애를 바라봄
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);//혹시 모를 뒤집어지는거방지 
        }
        yield return null;
    }
    [PunRPC]
    public void ShootLazer()// 레이져 쏘게 지시 하는 함수 
    {
        //이미 실행 중인 Lazer 코루틴이 있으면 중지
        if (lazerCoroutine != null)
        {
            StopCoroutine(lazerCoroutine);
            lazerCoroutine = null;
        }
        lazerCoroutine = StartCoroutine(Lazer());//레이저를 쏨 (코루틴)
    }


    IEnumerator Lazer()
    {

        yield return new WaitForSeconds(3f);//3초간 기달렸다가 
        ChargeParticles.gameObject.SetActive(true);//기를 모음 파티클 활성화 

        yield return new WaitUntil(() => !ChargeParticles.IsAlive(true));//기를 모으는 파티클이 끝나면 

        animator.SetBool("action", true);//애니메이션 bool action 실행
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("anim_MON006_ShootStart") && !animator.IsInTransition(0)); //anim_MON006_ShootStart애니메이션 상태인지 체크
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);//anim_MON006_ShootStart애니메이션이 끝났는지 체크

        yield return StartCoroutine(LoopLazer());

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("anim_MON006_Disappear") && !animator.IsInTransition(0));//anim_MON006_Disappear애니메이션 상태인지 체크
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);//anim_MON006_Disappear애니메이션이 끝났는지 체크

        lazerCoroutine = null;//코루틴 초기화 

        if (PhotonNetwork.IsMasterClient)//마스터클라이언트만
        {
            PoolManager.Instance.DespawnNetworked(gameObject);//몹제거 
        }

    }

    IEnumerator LoopLazer()//레이전 발사중인 코루틴
    {
        ShootParticles.gameObject.SetActive(true);//진짜레이저 활성화 
        yield return new WaitForSeconds(LazerLoopTime);// 레이저 시속시간 
        ShootParticles.gameObject.SetActive(false);//레이져 끄기 

        animator.SetTrigger("disappear");//레이콕 사라지는 애니메이션 발동
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)//빙결상태라고 하면 
        {
            ImFreeze = isFreeze;//상태 저장
            animator.speed = 0f;//애니메이션 정지
            IsFreeze.SetActive(true);//얼음 프리팹 활성화
        }
        else if (isFreeze == false)//빙결 상태 꺼지면 
        {
            Debug.Log(isFreeze + " 프리즈 해제");// 이거 넘어 오긴하는건가
            animator.SetTrigger("Freeze");//얼었을경우 필요한 애니메이션처리
            ImFreeze = isFreeze;//빙결상태 저장 
            StartCoroutine(FreezeCor());//해동 코루틴
        }
        else
        {
            Debug.Log("슉슉이 프리즈 고장남");
        }
    }

    public override void Move()//외부에서 해동 
    {
        StartCoroutine(FreezeCor());//해동 코루틴실행
    }

    IEnumerator FreezeCor()//해동코루틴 
    {
        yield return new WaitForSeconds(FreezeTime);//해동시간
        animator.speed = 1f;//애니메이션 실행 
        IsFreeze.SetActive(false);//얼음 프리팹 끄기 
    }


}

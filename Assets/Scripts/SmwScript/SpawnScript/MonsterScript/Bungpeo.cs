//범퍼
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bungpeo : EnemyBase
{

    [SerializeField] float explosionForce = 20f;//폭발시 힘크기
    [SerializeField] float explosionRadius = 20f;//폭발시 범위 
    [SerializeField] float upwardsModifier = 1f;//폭발시위로주는힘
    [SerializeField] LayerMask explosionMask;// 폭발을 적용 받을 레이어 

    [SerializeField] GameObject[] Fragment;//폭발을젹용받을 파편들

    [SerializeField] GameObject explosionPartycle;//폭발 파티클

    [SerializeField] GameObject[] Body;// 폭발시 자신 몸 메쉬스킨을 꺼준

    Animator animator;//애니메이션 변수

    Collider myCollider;//자기충돌체 받는 변수
    string AppearAni = "anim_MON003_appear";// 애니메이션 스테이트 이름 
    string explodeStateName = "anim_MON003_ready02";// 애니메이션 스테이트 이름 

    [SerializeField] GameObject CrashBunpeo;// 폭발전 사망시 사용되는 파티클 

    Terrain terrain;//지형값

    int IsActivateFragment = 0;//파편들이 다터졌는지 체크용

    [SerializeField] GameObject IsFreeze;//얼음상태 

    [SerializeField] float FreezeTime = 3;//해동시간 변수

    private void OnTriggerEnter(Collider other)// 충돌
    {
        if (!photonView.IsMine) return;//객체가 나의 것이 아니면 함수종료 
        if (other.CompareTag("Player"))//충돌체 테크가 플레이어면 
        {
            if (ImFreeze == true)//내가얼음상태면 
            {
                ImFreeze = false;//얼음 해제 
                StartCoroutine(FreezeCor());//해동 코루틴
            }
            else if (ImFreeze == false)//만약 얼음상태가 아니다 
            {
                damage = 1;//일단 데미지 1 
                Manager.Instance.observer.HitPlayer(damage);//플레이어에게 데미지 주는 옵저버

                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
                Instantiate(CrashBunpeo, hitPoint, rot);//폭발전 죽는 파티클 생성


                if (PhotonNetwork.IsMasterClient)//마스터클라이언트만 제거해줄꺼임 (중복 제거 방지)
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }

            }
        }

        else if (other.CompareTag("Monster"))//테크가 몬스터면 
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>();//몬스터들은 EnemyBase 가 있어서 otherEnemy 로 받아줌 

            if (otherEnemy == null)//EnemyBase 혹시 없을걸 대비해서 만들어놓은거
            {
                Debug.Log("몬스터 EnemyBase 가 없음");
                return;
            }

            if (ImFreeze == false && otherEnemy.ImFreeze == true)//내가 얼음상태가 아니고 상대가 얼음상태면 
            {

                otherEnemy.ImFreeze = false;//상대 얼음상태 꺼주고 
                otherEnemy.Move();//해동시켜줌

                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
                Instantiate(CrashBunpeo, hitPoint, rot);//폭발전 죽는 파티클 생성


                if (PhotonNetwork.IsMasterClient)//마스터 클라이언트만 
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);//몬스터를 제거 
                }

            }
        }

        else if (other.CompareTag("Aube"))// 테그가 오브일경우 (왠만하면 터질일이 없긴한데 체크)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
            Vector3 normal = (hitPoint - transform.position).normalized;// 몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
            Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
            Instantiate(CrashBunpeo, hitPoint, rot);//폭발전 죽는 파티클 생성

            if (PhotonNetwork.IsMasterClient)//마스터 클라이언트만 
            {
                PoolManager.Instance.DespawnNetworked(gameObject);//몹 제거 
            }

        }

    }

    public override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();//자신의 애니메이션 담기
        myCollider = GetComponent<Collider>();//자신의 충돌체 담기 


        terrain = Terrain.activeTerrain;//트레인 값 받아오기 

        float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f - 0.5f;// 범퍼 높이 값
        transform.position = new Vector3(transform.position.x, terrainY, transform.position.z);//높이값 지정

        for (int i = 0; i < Body.Length; i++)// 메쉬스킨 꺼져있으면 다시키기 
        {
            Body[i].SetActive(true);
        }


        for (int i = 0; i < Fragment.Length; i++)//파편들 꺼져있으면 다시 키기
        {
            Fragment[i].SetActive(true);
        }

        StartCoroutine(WaitAndExplode());//폭발 준비 코루틴 실행 
    }

    private IEnumerator WaitAndExplode()//폴발준비 코루틴 
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));// AppearAni 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))// 지금 플레이 중인 애니메이션이 AppearAni 라는 이름일 동안 계속 반복하라
            yield return null;//다음 프레임으로 넘기기

        myCollider.enabled = true;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(explodeStateName));// explodeStateName 애니메이션이 시작될 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)// 지금 플레이 중인 애니메이션이 explodeStateName 라는 이름일 동안 계속 반복하라
            yield return null;//다음 프레임으로 넘기기

        Explode();//폭발
    }


    public override void OnDisable()//비활성화 시 검사 (현재는 풀링에서 생성 제거 밖에 안하므로 작동은 안함)
    {
        base.OnDisable();
        for (int i = 0; i < Fragment.Length; i++)
        {

            if (Fragment[i].GetComponent<Collider>() != null)//파편들 충돌체 체크
            {
                Fragment[i].GetComponent<Collider>().enabled = false;//충돌체를꺼준다 (원레 꺼져있음 )
            }

            if (Fragment[i].GetComponent<Rigidbody>() != null)// 파편들 리지드바디 체크
            {
                Fragment[i].GetComponent<Rigidbody>().isKinematic = true;// 물리 엔진의 힘을 안받게함
                Fragment[i].GetComponent<Rigidbody>().useGravity = false;// 중력을 꺼준
            }
            Fragment[i].transform.localPosition = Vector3.zero;//자기 원레 위치로 다시되돌림
            Fragment[i].SetActive(true);//파편들을 다시켜줌
        }
        for (int i = 0; i < Body.Length; i++)//본체 메쉬 스킨 켜주기 
        {
            Body[i].SetActive(true);
        }
        myCollider.enabled = false;// 본체 충돌꺼주기
    }

    public void Explode()// 폭발 함수 
    {

        Vector3 explosionPosition = transform.position;

        for (int i = 0; i < Fragment.Length; i++)//파편들 관련처리
        {

            if (Fragment[i].GetComponent<Collider>() != null)//있는지 체크
            {
                Fragment[i].GetComponent<Collider>().enabled = true;//충돌체 켜주고 
            }

            if (Fragment[i].GetComponent<Rigidbody>() != null)//있는지 체크
            {
                Fragment[i].GetComponent<Rigidbody>().isKinematic = false;//물리엔진 힘을 받게하고 
                Fragment[i].GetComponent<Rigidbody>().useGravity = true;//중력을 켜줌
            }
            Fragment[i].transform.localPosition = Vector3.zero;
        }

        //Physics.OverlapSphere 는 지정한 위치(explosionPosition)와 반지름(explosionRadius)**으로 이루어진 구 안에 들어오는 모든 Collider를 반환합니다.
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius, explosionMask);// 폭발에 영향 받으것들을 가져옴

        foreach (Collider hit in colliders)//위에서 찾은 각 충돌체(hit)에 대해 처리하는것
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();//충돌된 물체들 리지드 바디값을 받아옴

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);// AddExplosionForce는 Rigidbody에 폭발력을 가하는 메서드 여기서 실징적으로 폭발의 힘을 줌
            }
        }

        Instantiate(explosionPartycle, transform.position, Quaternion.identity);//본체는 폭발 해야하므로 파티클 생성

        for (int i = 0; i < Body.Length; i++)// 본체가 터지면 몸은 보이면 안되니깐 메쉬스킨 이랑 충돌체 제거
        {
            Body[i].SetActive(false);//스킨 메쉬있는데 비활성화 
            myCollider.enabled = false;//충돌체제거
        }


    }


    [PunRPC]
    public void IsActivateRPC()//파편이 다 제거되었는지 체크하는 함수
    {
        //이거 파편 관련 된거임
        IsActivateFragment++;
        if (IsActivateFragment == 4)//파편이 다제거되면 
        {

            IsActivateFragment = 0;//값초기화 


            if (PhotonNetwork.IsMasterClient)//마스터클아이언트만
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }

        }
    }

    public override void Move()
    {
        StartCoroutine(FreezeCor());//해동쿠르틴
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction, isFreeze);
    }


    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)// 얼음상태가 되라고하면 
        {
            ImFreeze = isFreeze;//일단 상태를 저장함
            animator.speed = 0f;//애니메이션을 멈춘다
            IsFreeze.SetActive(true);//얼음 상태 프리펩을 화성화 
        }
        else if (isFreeze == false)//얼음 해제 명령이오면 
        {
            Debug.Log(isFreeze + " 프리즈 해제");// 이거 넘어 오긴하는건가
            ImFreeze = isFreeze;//상태저장
            StartCoroutine(FreezeCor());//해동코루틴
        }
        else
        {
            Debug.Log("범퍼 프리즈 고장남");
        }
    }
    IEnumerator FreezeCor()//해동코루틴
    {
        yield return new WaitForSeconds(FreezeTime);//해동하는시간
        animator.speed = 1f;//애니메이션 제생
        IsFreeze.SetActive(false);//얼음 프리팹 비활성화
    }

}

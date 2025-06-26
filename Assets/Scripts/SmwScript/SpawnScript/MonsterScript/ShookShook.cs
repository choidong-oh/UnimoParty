using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ShookShook : EnemyBase
{

    Vector3 Position;// 시작 위치나 이동 중 계산용 위치를 저장할 변수
    Vector3 Terrainpos;// Terrain의 월드 위치를 저장할 변수
    Vector3 Terrainsize;// Terrain 데이터의 크기를 저장할 변수

    Vector3 Target;// 이동 목표 지점을 저장할 변수

    float MoveSpeed = 5f;// 이동 속도
    Terrain terrain;// 현재 활성화된 Terrain 컴포넌트를 저장합니다.

    float minX;// 지형의 최소 X 좌표
    float maxX;// 지형의 최대 X 좌표
    float minZ;// 지형의 최소 Z 좌표
    float maxZ;// 지형의 최대 Z 좌표
    public float fixedY;// Y 위치 보정용 오프셋

    Coroutine Coroutine;// 이동 코루틴을 저장할 변수

    [SerializeField] GameObject CrashShookShook;// 슉슉이 죽을때 쓰는 파티클
    Collider myCollider;// 충돌체 자기자신 저장할것 

    float MoveSpeedSave;// 얼음 상태 전후 속도 복원을 위한 변수

    Animator animator;//자기자신의 Animator 컴포넌트를 넣을 animator 변수

    [SerializeField] float FreezeTime = 3;// 얼음 상태 지속 시간
    [SerializeField] GameObject IsFreeze;// 얼음 프리팹

    public override void OnEnable()              
    {
        base.OnEnable();                     
        myCollider = GetComponent<Collider>();// 자기자신의 Collider 컴포넌트를 가져와 저장
        myCollider.enabled = false;// 충돌체 꺼줌

        terrain = Terrain.activeTerrain;// 씬에서 활성화된 Terrain 컴포넌트 가져오기
        Terrainsize = terrain.terrainData.size;// Terrain의 크기 정보 저장
        Terrainpos = terrain.transform.position;// Terrain의 월드 위치 저장

        Position = transform.position;// 현재 오브젝트 위치를 Position에 저장

        minX = Terrainpos.x;// 지형의 최소 X
        maxX = Terrainpos.x + Terrainsize.x;// 지형의 최대 X
        minZ = Terrainpos.z;// 지형의 최소 Z
        maxZ = Terrainpos.z + Terrainsize.z;// 지형의 최대 Z

        // 오브젝트와 네 면 간 거리 계산
        float Left = Mathf.Abs(Position.x - minX);
        float Right = Mathf.Abs(Position.x - maxX);
        float Bottom = Mathf.Abs(Position.z - minZ);
        float Top = Mathf.Abs(Position.z - maxZ);

        // 네 거리 중 가장 가까운 면까지의 거리
        float NearPos = Mathf.Min(Left, Right, Bottom, Top);

        if (NearPos == Left)// 왼쪽 면이 가장 가깝다면
        {
            Position.x = minX;// 시작 위치를 왼쪽 경계로 설정
            Target = new Vector3(maxX, 0, Position.z); // 목표는 오른쪽  모서리로 설정
        }
        else if (NearPos == Right)// 오른쪽 면이 가장 가깝다면
        {
            Position.x = maxX;// 시작 위치를 오른쪽 경계로 설정
            Target = new Vector3(minX, 0, Position.z); // 목표는 왼쪽 모서리로 설정
        }
        else if (NearPos == Bottom)// 아래 면이 가장 가깝다면
        {
            Position.z = minZ;// 시작 위치를 아래 모서리로 설정
            Target = new Vector3(Position.x, 0, maxZ); // 목표는 위쪽 모서리로 설정
        }
        else if (NearPos == Top)// 위 면이 가장 가깝다면
        {
            Position.z = maxZ;// 시작 위치를 위 경계로 설정
            Target = new Vector3(Position.x, 0, minZ); // 목표는 아래 모서리로 설정
        }
        else
        {
            Debug.Log("슉슉이 절대값 잡아주는곳 오류임");
        }

        // Y 위치를 지형 높이에 맞춰 보정
        float terrainY = terrain.SampleHeight(Position) + transform.localScale.y / 2f + fixedY;
        Position.y = terrainY;
        transform.position = Position;// 높이적용

        terrainY = terrain.SampleHeight(Target) + transform.localScale.y / 2f + fixedY;
        Target.y = terrainY;// 목표 지점도 Y 높이적용

        transform.position = Position;//시작지점으로 좌표지정

        Coroutine = StartCoroutine(GoShookShook());//이동 코루틴 실행
    }

    public override void OnDisable() 
    {
        base.OnDisable(); 
        if (Coroutine != null)               
        {
            StopCoroutine(Coroutine);             
            Coroutine = null;                     
        }
    }

    IEnumerator GoShookShook() 
    {
        Vector3 pos = transform.position;
        myCollider.enabled = true;// 충돌체 키기  
        Target.y = pos.y;// 목표 Y는 현재 Y 유지             

        // 목표에 거의 도달할 때까지 반복
        while (Vector3.Distance(transform.position, Target) > 0.5f)
        {
            transform.LookAt(Target); //Target을 바라보게 함 
            transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime; // 앞으로 가는 코드 

            // 지형 높이에 맞게 Y 보정
            float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;
            pos = transform.position;// 현제 위치에서
            pos.y = terrainY;//y 적용해야해서 
            transform.position = pos;// 이동적용 

            yield return new WaitForFixedUpdate();// FixedUpdate 주기마다 대기
        }

        transform.position = Target;// 정확히 목표 지점 고정시켜버림(도착했으니깐)
        if (PhotonNetwork.IsMasterClient)
        {
            PoolManager.Instance.DespawnNetworked(gameObject); //PoolManager으로 반환
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;// 소유 클라이언트가 아니면 무시

        if (other.CompareTag("Player"))// 플레이어와 충돌했을 때
        {
            if (ImFreeze == true)// 자신이 얼어있으면
            {
                ImFreeze = false;// 얼음 해제
                StartCoroutine(FreezeCor());// 얼음 해제 코루틴 실행
            }
            else if (ImFreeze == false)// 얼어있지 않으면
            {
                damage = 1;// 데미지 설정
                Manager.Instance.observer.HitPlayer(damage);// 플레이어에 데미지 전달

                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;//몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
                Instantiate(CrashShookShook, hitPoint, rot);  // 슉슉이 죽는 파티클 생성

                if (PhotonNetwork.IsMasterClient)   
                {
                    PoolManager.Instance.DespawnNetworked(gameObject); // PoolManager에 반환
                }
            }
        }
        else if (other.CompareTag("Monster"))// 다른 몬스터와 충돌했을 때
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>(); // EnemyBase 가져오기 다른몬스터도 EnemyBase 있어서 
            if (otherEnemy == null)// 컴포넌트 없으면 경고
            {
                Debug.Log("몬스터 EnemyBase 가 없음");
                return;
            }

            if (ImFreeze == false && otherEnemy.ImFreeze == true) // 자신얼음아니고 상대가 얼음 상태면
            {
                otherEnemy.ImFreeze = false;// 충돌된 몬스터 얼음 해제
                otherEnemy.Move();// 상대 Move() 호출해서 해동을 킴

                Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
                Vector3 normal = (hitPoint - transform.position).normalized;//몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
                Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
                Instantiate(CrashShookShook, hitPoint, rot); // 슉슉이 죽는 파티클 생성

                if (PhotonNetwork.IsMasterClient)   // 마스터 클라이언트일 때만 제거
                {
                    PoolManager.Instance.DespawnNetworked(gameObject);
                }
            }
        }
        else if (other.CompareTag("Aube"))        // Aube 태그와 충돌했을 때
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);//이 오브젝트와 가장 가까운 지점을 계산
            Vector3 normal = (hitPoint - transform.position).normalized;//몬스터 중심에서 충돌 지점으로 향하는 방향 벡터를 단위 벡터로 변환
            Quaternion rot = Quaternion.LookRotation(normal);// 회전방향 지정
            Instantiate(CrashShookShook, hitPoint, rot); // 슉슉이 죽는 파티클 생성

            if (PhotonNetwork.IsMasterClient)       // 마스터 클라이언트일 때만 제거
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }
        }
    }

    public override void Move()
    {
        StartCoroutine(FreezeCor());// 얼음 해제 코루틴 실행
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction, isFreeze);
    }

    [PunRPC]                                   
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)// 얼음 상태 일때
        {
            IsFreeze.SetActive(true);// 얼음 프리팹 활성화
            ImFreeze = isFreeze;// 상태 갱신
            MoveSpeedSave = MoveSpeed;// 현재 속도 저장
            MoveSpeed = 0;// 이동 속도 0으로 설정
            animator.speed = 0f;// 애니메이션 재생 멈춤
        }
        else if (isFreeze == false)// 얼음 상태 해재 일때
        {
            ImFreeze = isFreeze;// 상태 플래그 갱신
            StartCoroutine(FreezeCor());// 일정 시간 후 복귀 코루틴 실행
        }
        else
        {
            Debug.Log("슉슉이 프리즈 고장남");
        }
    }

    IEnumerator FreezeCor()// 얼음 상태 지속 후 자동 해제 코루틴
    {
        yield return new WaitForSeconds(FreezeTime); // 지정된 시간만큼 대기
        MoveSpeed = MoveSpeedSave;// 원래 이동 속도 복원
        animator.speed = 1f;// 애니메이터 다시재생
        IsFreeze.SetActive(false);// 얼음 프리팹 비활성화
    }

} 

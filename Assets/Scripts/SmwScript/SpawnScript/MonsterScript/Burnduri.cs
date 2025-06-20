using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnduri : EnemyBase
{

    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    Vector3 Target;
    Vector3 myPos;

    [Header("이동 설정")]
    [SerializeField] float MoveSpeed = 1f;
    [SerializeField] float chargeSpeed = 10f;
    [SerializeField] float chargeDistance = 3f;

    [Header("거리 조건")]
    [SerializeField] float triggerDistance = 10f;
    [SerializeField] float fixedY = 0f;


    private bool isCharging = false;


    Terrain terrain;

    private Coroutine updateRoutine;
    private Coroutine moveRoutine;

    Transform nearestPlayer;

    int dashStateHash;


    Animator animator;
    Collider myCollider;

    //private AnimationClip appearanceClip;
    //private AnimationClip encounterClip;
    //private AnimationClip disappearClip;

    [SerializeField] GameObject CrashBurnduri;

    [SerializeField] float FreezeTime = 3;

    [SerializeField] GameObject IsFreeze;

    string AppearAni = "anim_01_MON001_Bduri_Appearance";
    string state2 = "anim_02_MON001_Bduri_SlowMove";
    string state3 = "anim_03_MON001_Bduri_Encounter";
    string state4 = "anim_04_MON001_Bduri_Dash";
    string state5 = "anim_01_MON001_Bduri_Disappearance";



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashBurnduri, hitPoint, rot);

            Manager.Instance.observer.HitPlayer(damage);
            StopAllCoroutines();
            PoolManager.Instance.Despawn(gameObject);
        }

        if (other.gameObject.tag == "Monster")
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>();
            if (otherEnemy == null)
                return;

            if (otherEnemy.ImFreeze)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);

                Vector3 normal = (hitPoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(normal);

                GameObject inst = Instantiate(CrashBurnduri, hitPoint, rot);

                PoolManager.Instance.Despawn(gameObject);
            }
        }

        if (other.gameObject.tag == "Aube")
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Vector3 normal = (hitPoint - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(normal);

            GameObject inst = Instantiate(CrashBurnduri, hitPoint, rot);

            PoolManager.Instance.Despawn(gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();

        myCollider.enabled = false;

        //한번만 찾을꺼임
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objs)
            {
                players.Add(obj.transform);
            }
        }

        terrain = Terrain.activeTerrain;
        isCharging = false;

        StartCoroutine(GoBurnduri());
    }

    IEnumerator GoBurnduri()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))
        yield return null;

        myCollider.enabled = true;

        updateRoutine = StartCoroutine(UpdateDistance());
        moveRoutine = StartCoroutine(MoveRoutine());
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
        PoolManager.Instance.Despawn(gameObject);
    }



    public override void CsvEnemyInfo()
    {

    }

    public override void Move(Vector3 direction)
    {
        photonView.RPC("MoveRPC", RpcTarget.All, direction);
    }

    [PunRPC]
    public void MoveRPC(Vector3 direction)
    {
        
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction,isFreeze);
    }

    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)
        {
            StopAllCoroutines();
            ImFreeze = isFreeze;
            IsFreeze.SetActive(true);
            myCollider.enabled = false;
            animator.speed = 0f;
        }
        else if (isFreeze == false)
        {
            //Move(direction);
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
        myCollider.enabled = true;
        animator.speed = 1f;
        StartCoroutine(GoBurnduri());
        IsFreeze.SetActive(false);

    }
}

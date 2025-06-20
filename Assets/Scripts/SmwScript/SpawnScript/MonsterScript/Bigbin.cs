using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bigbin : EnemyBase
{

    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    [Header("이동 설정")]
    float MoveSpeed = 5f;
    float FirstSpeed;


    [Header("거리 조건")]
    [SerializeField] float fixedY = 0f;

    Terrain terrain;

    Collider myCollider;

    [SerializeField] GameObject CrashBigbin;

    [SerializeField] GameObject JumpParticles;

    [SerializeField] GameObject JumpExplode;

    Transform nearestPlayer;

    Vector3 myPos;
    Vector3 Target;

    Animator animator;

    [SerializeField] float FreezeTime = 3;

    string AppearAni = "anim_MON004_appear";
    string state2 = "anim_MON004_readytojump";
    string state3 = "anim_MON004_jump01";
    string state4 = "anim_MON004_jump02";
    string state5 = "anim_MON004_jump03";

    [SerializeField] GameObject IsFreeze;

    float MoveSpeedSave;

    public override void OnEnable()
    {
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
        terrain = Terrain.activeTerrain;
        FirstSpeed = MoveSpeed / 2;
        base.OnEnable();
        StartCoroutine(GoBigBin());

    }


    public override void OnDisable()
    {
        base.OnDisable();
        MoveSpeed = FirstSpeed * 2;
        myCollider.enabled = false;
    }


    IEnumerator GoBigBin()
    {


        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))
            yield return null;

        myCollider.enabled = true;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state2));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state2))
            yield return null;
        JumpParticles.SetActive(true);
        MoveSpeed += FirstSpeed;


        StartCoroutine(MoveRoutine());
        StartCoroutine(UpdateDistance());

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state3));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state3))
            yield return null;
        JumpParticles.SetActive(true);

        MoveSpeed += FirstSpeed;





        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state4));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(state4))
            yield return null;

        JumpParticles.SetActive(true);

        MoveSpeed += FirstSpeed;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(state5));
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        Instantiate(JumpExplode, transform.position, Quaternion.identity);
        StopAllCoroutines();
        PoolManager.Instance.Despawn(gameObject);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Manager.Instance.observer.HitPlayer(damage);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashBigbin, hitPoint, rot);


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

                GameObject inst = Instantiate(CrashBigbin, hitPoint, rot);

                PoolManager.Instance.Despawn(gameObject);
            }
        }

        if (other.gameObject.tag == "Aube")
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Vector3 normal = (hitPoint - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(normal);

            GameObject inst = Instantiate(CrashBigbin, hitPoint, rot);

            PoolManager.Instance.Despawn(gameObject);
        }

    }

    IEnumerator MoveRoutine()
    {

        while (true)
        {
            float CheckNear = Vector3.Distance(myPos, Target);

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


    public override void Move(Vector3 direction)
    {
        photonView.RPC("MoveRPC", RpcTarget.All, direction);
    }

    [PunRPC]
    public void MoveRPC(Vector3 direction)
    {
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
        terrain = Terrain.activeTerrain;
        FirstSpeed = MoveSpeed / 2;
        base.OnEnable();
        StartCoroutine(GoBigBin());
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
            myCollider.enabled = false;
            ImFreeze = isFreeze;
            IsFreeze.SetActive(true);
            MoveSpeedSave = MoveSpeed;
            MoveSpeed = 0;
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
            Debug.Log("빅빈 프리즈 고장남");
        }
    }

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator FreezeCor()
    {
        yield return new WaitForSeconds(FreezeTime);
        MoveSpeed = MoveSpeedSave;
        animator.speed = 1f;
        myCollider.enabled = true;
        IsFreeze.SetActive(false);
    }
}

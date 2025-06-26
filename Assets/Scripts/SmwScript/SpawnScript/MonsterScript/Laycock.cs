//레이콕
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laycock : EnemyBase
{

    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    Vector3 myPos;
    Vector3 Target;

    Transform nearestPlayer;

    [SerializeField] ParticleSystem ChargeParticles;
    [SerializeField] ParticleSystem ShootParticles;
    [SerializeField] GameObject DieParticles;

    Terrain terrain;

    float LazerLoopTime = 3;

    Animator animator;

    Collider myCollider;
    string AppearAni = "anim_MON006_Appear";

    [SerializeField] float FreezeTime = 3;

    [SerializeField] GameObject IsFreeze;

    private Coroutine lazerCoroutine;

    LaycockSP laycockSP;

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
                Instantiate(DieParticles, hitPoint, rot);

                laycockSP.DisCountLaycock();


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
                Instantiate(DieParticles, hitPoint, rot);

                laycockSP.DisCountLaycock();


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

            Instantiate(DieParticles, hitPoint, rot);


            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }

        }

    }


    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Distance());
    }


    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        myCollider.enabled = false;
    }

    IEnumerator Distance()
    {
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
        terrain = Terrain.activeTerrain;
        laycockSP = FindObjectOfType<LaycockSP>();

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))
            yield return null;

        myCollider.enabled = true;

        float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f;
        transform.position = new Vector3(transform.position.x, terrainY, transform.position.z);
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objs)
            {
                players.Add(obj.transform);
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
        yield return null;
    }
    [PunRPC]
    public void ShootLazer()
    {
        //이미 실행 중인 Lazer 코루틴이 있으면 중지
        if (lazerCoroutine != null)
        {
            StopCoroutine(lazerCoroutine);
            lazerCoroutine = null;
        }
        lazerCoroutine = StartCoroutine(Lazer());
    }


    IEnumerator Lazer()
    {

        yield return new WaitForSeconds(3f);
        ChargeParticles.gameObject.SetActive(true);

        yield return new WaitUntil(() => !ChargeParticles.IsAlive(true));

        animator.SetBool("action", true);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("anim_MON006_ShootStart") && !animator.IsInTransition(0));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        yield return StartCoroutine(LoopLazer());

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("anim_MON006_Disappear") && !animator.IsInTransition(0));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        lazerCoroutine = null;

        if (PhotonNetwork.IsMasterClient)
        {
            PoolManager.Instance.DespawnNetworked(gameObject);
        }

    }

    IEnumerator LoopLazer()
    {
        ShootParticles.gameObject.SetActive(true);
        yield return new WaitForSeconds(LazerLoopTime);
        ShootParticles.gameObject.SetActive(false);

        animator.SetTrigger("disappear");
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)
        {
            ImFreeze = isFreeze;
            animator.speed = 0f;
            IsFreeze.SetActive(true);
        }
        else if (isFreeze == false)
        {
            Debug.Log(isFreeze + " 프리즈 해제");// 이거 넘어 오긴하는건가
            animator.SetTrigger("Freeze");
            ImFreeze = isFreeze;
            StartCoroutine(FreezeCor());
        }
        else
        {
            Debug.Log("슉슉이 프리즈 고장남");
        }
    }

    public override void Move()
    {
        StartCoroutine(FreezeCor());
    }

    IEnumerator FreezeCor()
    {
        yield return new WaitForSeconds(FreezeTime);
        animator.speed = 1f;
        IsFreeze.SetActive(false);
    }


}

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            damage = 1;
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(DieParticles, hitPoint, rot);

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

                GameObject inst = Instantiate(DieParticles, hitPoint, rot);


                Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
                Manager.Instance.observer.HitPlayer(damage);

                PoolManager.Instance.Despawn(gameObject);
            }
        }

        if (other.gameObject.tag == "Aube")
        {
            Manager.Instance.observer.HitPlayer(damage);

            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Vector3 normal = (hitPoint - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(normal);

            GameObject inst = Instantiate(DieParticles, hitPoint, rot);

            PoolManager.Instance.Despawn(gameObject);
        }

    }


    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Distance());;
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
        StartCoroutine(Lazer());
    }


    IEnumerator Lazer()
    {

        yield return new WaitForSeconds(3f);
        ChargeParticles.gameObject.SetActive(true);

        yield return new WaitUntil(() => !ChargeParticles.IsAlive(true));

        animator.SetTrigger("action");
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("anim_MON006_ShootStart") && !animator.IsInTransition(0));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        yield return StartCoroutine(LoopLazer());

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("anim_MON006_Disappear") && !animator.IsInTransition(0));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);


        PoolManager.Instance.Despawn(gameObject);
    }

    IEnumerator LoopLazer()
    {
        ShootParticles.gameObject.SetActive(true);
        yield return new WaitForSeconds(LazerLoopTime);
        ShootParticles.gameObject.SetActive(false);
        
        animator.SetTrigger("disappear");
    }




    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)
        {
            ImFreeze = isFreeze;
            myCollider.enabled = false;
            animator.speed = 0f;
            IsFreeze.SetActive(true);
        }
        else if (isFreeze == false)
        {
            ImFreeze = isFreeze;
            StartCoroutine(FreezeCor());
        }
        else
        {
            Debug.Log("슉슉이 프리즈 고장남");
        }
    }

    public override void Move(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator FreezeCor()
    {
        yield return new WaitForSeconds(FreezeTime);
        animator.speed = 1f;
        myCollider.enabled = true;
        IsFreeze.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bigbin : EnemyBase
{
    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    [Header("이동 설정")]
    [SerializeField] float MoveSpeed = 1f;
    float FirstSpeed;


    [Header("거리 조건")]
    [SerializeField] float fixedY = 0f;

    Terrain terrain;

    Collider myCollider;

    [SerializeField] GameObject CrashBigbin;

    [SerializeField] GameObject JumpParticles;

    Transform nearestPlayer;

    Vector3 myPos;
    Vector3 Target;

    Animator animator;

    string state1 = "anim_MON004_appear";
    string state2 = "anim_MON004_readytojump";
    string state3 = "anim_MON004_jump01";
    string state4 = "anim_MON004_jump02";
    string state5 = "anim_MON004_jump03";

    private void Start()
    {
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
        FirstSpeed = MoveSpeed / 2;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(GoBigBin());

    }


    public override void OnDisable()
    {
        base.OnDisable();
        MoveSpeed = FirstSpeed;
    }


    IEnumerator GoBigBin()
    {

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(state3)
        );
        myCollider.enabled = true;
        StartCoroutine(MoveRoutine());
        StartCoroutine(UpdateDistance());
        yield return new WaitUntil(() =>
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName(state3) && info.normalizedTime >= 1f;
        });
        MoveSpeed = MoveSpeed + FirstSpeed;

        yield return new WaitUntil(() =>
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName(state4) && info.normalizedTime >= 1f;
        });

        MoveSpeed = MoveSpeed + FirstSpeed;

        yield return new WaitUntil(() =>
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName(state5) && info.normalizedTime >= 1f;
        });
        
        gameObject.SetActive(false);

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(JumpParticles, hitPoint, rot);
        }


        if (other.gameObject.tag == "Player")
        {
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashBigbin, hitPoint, rot);


            gameObject.SetActive(false);
        }

    }

    IEnumerator MoveRoutine()
    {

        while (true)
        {
            float CheckNear = Vector3.Distance(myPos, Target);

            myPos = transform.position;
            float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;
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
            }


            yield return new WaitForSeconds(0.5f);
        }
    }

    public override void Move(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        throw new System.NotImplementedException();
    }

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }
}

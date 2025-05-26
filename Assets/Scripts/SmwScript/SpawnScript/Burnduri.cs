using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnduri : EnemyBase
{
    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    Vector3 Target;

    [Header("이동 설정")]
    public float MoveSpeed = 1f;
    public float chargeSpeed = 10f;
    public float chargeDistance = 3f;

    [Header("거리 조건")]
    public float triggerDistance = 10f;
    public float fixedY = 0f;

    private Transform currentTarget;
    private bool isCharging = false;


    Terrain terrain;

    private Coroutine updateRoutine;
    private Coroutine moveRoutine;

    Animator animator;
    Collider col;

    private void Awake()
    {
        //한번만 찾을꺼임
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objs)
            {
                players.Add(obj.transform);
            }
        }
    }

    public override void CsvEnemyInfo()
    {

    }

    public override void Move(Vector3 direction)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
            Manager.Instance.observer.HitPlayer(damage);
            animator.SetTrigger("disappear");
        }
    }

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        col.enabled = false;
        currentTarget = transform;
        terrain = Terrain.activeTerrain;
        isCharging = false;

        StartCoroutine(SpawnBurnduiri());

        //updateRoutine = StartCoroutine(UpdateDistance());
        //moveRoutine = StartCoroutine(MoveRoutine());
    }


    void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator SpawnBurnduiri()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)

            yield return null;

        animator.SetTrigger("Move");

        updateRoutine = StartCoroutine(UpdateDistance());
        moveRoutine = StartCoroutine(MoveRoutine());

        yield return null;
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

            Vector3 myPos = transform.position;
            Transform nearestPlayer = null;
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
                currentTarget = nearestPlayer;
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
                Vector3 myPos = transform.position;
                float CheckNear = Vector3.Distance(myPos, Target);

                if (CheckNear < chargeDistance)
                {
                    isCharging = true;
                    StopCoroutine(updateRoutine);
                    StopCoroutine(moveRoutine);
                    StartCoroutine(ChargeRoutine());
                    yield break;
                }
                else
                {
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
        StopCoroutine(updateRoutine);
        StopCoroutine(moveRoutine);

        animator.SetTrigger("ChargeStart");

        yield return new WaitForSeconds(1f);

        Vector3 startPos = transform.position;
        Vector3 dir = transform.forward;  // 이 순간의 방향을 저장

        animator.SetTrigger("Charge");

        while (Vector3.Distance(transform.position, startPos) < triggerDistance)
        {
            float terrainY = terrain.SampleHeight(transform.position);
            terrainY += transform.localScale.y / 2f;
            dir.y = terrainY + fixedY;
            transform.position += dir * chargeSpeed * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        animator.SetTrigger("disappear");

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)

            yield return null;

        enabled = false;

        Destroy(gameObject);
    }

}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnduri : EnemyBase
{
    [HideInInspector] public GameObject prefab;

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

    private AnimationClip appearanceClip;
    private AnimationClip encounterClip;
    private AnimationClip disappearClip;

    [SerializeField] GameObject CrashBurnduri;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashBurnduri, hitPoint, rot);


            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
            Manager.Instance.observer.HitPlayer(damage);
            StopAllCoroutines();
            PoolManager.Instance.Despawn(prefab, gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();

        myCollider.enabled = false;

        if (animator != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "anim_01_MON001_Bduri_Appearance")
                    appearanceClip = clip;
                else if (clip.name == "anim_03_MON001_Bduri_Encounter")
                    encounterClip = clip;
                else if (clip.name == "anim_01_MON001_Bduri_Disappearance")
                    disappearClip = clip;
            }
        }
        // 컨트롤러에 등록된 모든 클립을 뒤져서 원하는 이름의 클립을 저장

        if (appearanceClip == null || encounterClip == null || disappearClip == null)
            Debug.LogWarning("Appearance 또는 Encounter 클립없음");

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
        float SpawnCool = appearanceClip != null ? appearanceClip.length / animator.speed : 0f;
        yield return new WaitForSeconds(SpawnCool);
        myCollider.enabled = true;
        updateRoutine = StartCoroutine(UpdateDistance());
        moveRoutine = StartCoroutine(MoveRoutine());
        yield return null;
    }


    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
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
        float ChargeCool = encounterClip != null ? encounterClip.length / animator.speed : 0f;
        yield return new WaitForSeconds(ChargeCool);

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
        animator.SetTrigger("disappear");
        float DisappearCool = disappearClip != null ? disappearClip.length / animator.speed : 0f;
        myCollider.enabled = false;
        yield return new WaitForSeconds(DisappearCool);
        PoolManager.Instance.Despawn(prefab, gameObject);
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
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();

        myCollider.enabled = false;

        if (animator != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "anim_01_MON001_Bduri_Appearance")
                    appearanceClip = clip;
                else if (clip.name == "anim_03_MON001_Bduri_Encounter")
                    encounterClip = clip;
                else if (clip.name == "anim_01_MON001_Bduri_Disappearance")
                    disappearClip = clip;
            }
        }
        // 컨트롤러에 등록된 모든 클립을 뒤져서 원하는 이름의 클립을 저장

        if (appearanceClip == null || encounterClip == null || disappearClip == null)
            Debug.LogWarning("Appearance 또는 Encounter 클립을 찾지 못했습니다.");

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
        }
        else if (isFreeze == false)
        {
            Move(direction);
        }
        else
        {
            Debug.Log("번드리 프리즈 고장남");
        }
    }

}

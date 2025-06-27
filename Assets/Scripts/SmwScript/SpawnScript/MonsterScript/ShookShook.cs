//슉슉
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ShookShook : EnemyBase
{

    Vector3 Position;
    Vector3 Terrainpos;
    Vector3 Terrainsize;

    Vector3 Target;

    float MoveSpeed = 5f;
    Terrain terrain;

    float minX;
    float maxX;
    float minZ;
    float maxZ;
    public float fixedY;

    Coroutine Coroutine;

    [SerializeField] GameObject CrashShookShook;
    Collider myCollider;

    float MoveSpeedSave;

    Animator animator;

    [SerializeField] float FreezeTime = 3;

    [SerializeField] GameObject IsFreeze;
    public override void OnEnable()
    {
        base.OnEnable();
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
        terrain = Terrain.activeTerrain;
        Terrainsize = terrain.terrainData.size;
        Terrainpos = terrain.transform.position;

        Position = transform.position;

        minX = Terrainpos.x;
        maxX = Terrainpos.x + Terrainsize.x;
        minZ = Terrainpos.z;
        maxZ = Terrainpos.z + Terrainsize.z;


        float Left = Mathf.Abs(Position.x - minX);
        float Right = Mathf.Abs(Position.x - maxX);
        float Bottom = Mathf.Abs(Position.z - minZ);
        float Top = Mathf.Abs(Position.z - maxZ);

        float NearPos = Mathf.Min(Left, Right, Bottom, Top);

        if (NearPos == Left)
        {
            Position.x = minX;
            Target = new Vector3(maxX, 0, Position.z);
        }
        else if (NearPos == Right)
        {
            Position.x = maxX;
            Target = new Vector3(minX, 0, Position.z);
        }
        else if (NearPos == Bottom)
        {
            Position.z = minZ;
            Target = new Vector3(Position.x, 0, maxZ);
        }
        else if (NearPos == Top)
        {
            Position.z = maxZ;
            Target = new Vector3(Position.x, 0, minZ);
        }
        else
        {
            Debug.Log("슉슉이 절대값 잡아주는곳 오류임");
        }

        float terrainY = terrain.SampleHeight(Position) + transform.localScale.y / 2f + fixedY;
        Position.y = terrainY;
        transform.position = Position;

        terrainY = terrain.SampleHeight(Target) + transform.localScale.y / 2f + fixedY;
        Target.y = terrainY;

        transform.position = Position;

        Coroutine = StartCoroutine(GoShookShook());
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
        myCollider.enabled = true;
        Target.y = pos.y;
        while (Vector3.Distance(transform.position, Target) > 0.5f)
        {
            transform.LookAt(Target);
            transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;
            float terrainY = terrain.SampleHeight(transform.position);
            terrainY += transform.localScale.y / 2f + fixedY;
            pos = transform.position;
            pos.y = terrainY;
            transform.position = pos;
            yield return new WaitForFixedUpdate();
        }
        transform.position = Target;
        if (PhotonNetwork.IsMasterClient)
        {
            PoolManager.Instance.DespawnNetworked(gameObject);
        }
    }

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
                Instantiate(CrashShookShook, hitPoint, rot);
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
                Instantiate(CrashShookShook, hitPoint, rot);


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

            Instantiate(CrashShookShook, hitPoint, rot);


            if (PhotonNetwork.IsMasterClient)
            {
                PoolManager.Instance.DespawnNetworked(gameObject);
            }

        }

    }



    public override void Move()
    {
        StartCoroutine(FreezeCor());
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction, isFreeze);
    }

    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)
        {
            IsFreeze.SetActive(true);
            ImFreeze = isFreeze;
            MoveSpeedSave = MoveSpeed;
            MoveSpeed = 0;
            animator.speed = 0f;
            
        }
        else if (isFreeze == false)
        {
            Debug.Log(isFreeze + " 프리즈 해제");// 이거 넘어 오긴하는건가
            ImFreeze = isFreeze;
            StartCoroutine(FreezeCor());
        }
        else
        {
            Debug.Log("슉슉이 프리즈 고장남");
        }
    }

    IEnumerator FreezeCor()
    {
        yield return new WaitForSeconds(FreezeTime);
        MoveSpeed = MoveSpeedSave;
        //animator.speed = 1f;
        IsFreeze.SetActive(false);
    }



}

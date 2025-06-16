using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ShookShook : EnemyBase
{
    [HideInInspector] public GameObject prefab;

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
        PoolManager.Instance.Despawn(prefab, gameObject);

    }

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

            GameObject inst = Instantiate(CrashShookShook, hitPoint, rot);

            PoolManager.Instance.Despawn(prefab, gameObject);

        }

    }


    public override void Move(Vector3 direction)
    {
        photonView.RPC("MoveRPC", RpcTarget.All, direction);
    }

    [PunRPC]
    public void MoveRPC(Vector3 direction)
    {
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

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
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
            MoveSpeedSave = MoveSpeed;
            MoveSpeed = 0;

        }
        else if (isFreeze == false)
        {
            MoveSpeed = MoveSpeedSave;
        }
        else
        {
            Debug.Log("슉슉이 프리즈 고장남");
        }
    }


}

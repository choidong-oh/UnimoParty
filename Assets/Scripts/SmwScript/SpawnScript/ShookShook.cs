using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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


    private void OnEnable()
    {
        terrain = Terrain.activeTerrain;
        Terrainsize = terrain.terrainData.size;
        Terrainpos = terrain.transform.position;

        Position = transform.position;
        Target = transform.position;//좌표 마추기용 

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
            Target.x = maxX;

        }
        else if (NearPos == Right)
        {
            Position.x = maxX;
            Target.x = minX;

        }
        else if (NearPos == Bottom)
        {
            Position.z = minZ;
            Target.z = maxZ;

        }
        else if (NearPos == Top)
        {
            Position.z = maxZ;
            Target.z = minZ;

        }
        else
        {
            Debug.Log("너는 왜 오류임?");
        }
        transform.position = Position;

        Coroutine = StartCoroutine(GoShookShook());
    }

    private void OnDisable()
    {
        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
            Coroutine = null;
        }
    }

    IEnumerator GoShookShook()
    {
        Vector3 pos = transform.position;
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
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            damage = 1;
            //Manager.Instance.observer.HitPlayer(damage);
            //Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashShookShook, hitPoint, rot);

            gameObject.SetActive(false);
        }



    }


    public override void Move(Vector3 direction)
    {
        terrain = Terrain.activeTerrain;
        Terrainsize = terrain.terrainData.size;
        Terrainpos = terrain.transform.position;

        Position = transform.position;
        Target = transform.position;//좌표 마추기용 

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
            Target.x = maxX;

        }
        else if (NearPos == Right)
        {
            Position.x = maxX;
            Target.x = minX;

        }
        else if (NearPos == Bottom)
        {
            Position.z = minZ;
            Target.z = maxZ;

        }
        else if (NearPos == Top)
        {
            Position.z = maxZ;
            Target.z = minZ;

        }
        else
        {
            Debug.Log("너는 왜 오류임?");
        }
        transform.position = Position;

        Coroutine = StartCoroutine(GoShookShook());
    }

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }
}

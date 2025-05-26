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



    private void OnEnable()
    {
        terrain = Terrain.activeTerrain;
        Terrainsize = terrain.terrainData.size;
        Terrainpos = terrain.transform.position;

        Position = transform.position;

        minX = Terrainpos.x;
        maxX = Terrainpos.x + Terrainsize.x;
        minZ = Terrainpos.z;
        maxZ = Terrainpos.z + Terrainsize.z;



        Position = transform.position;
        Target = transform.position;

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
        while (Vector3.Distance(transform.position, Target) > 0.5f)
        {
            transform.LookAt(Target);
            transform.position += transform.forward * MoveSpeed * Time.fixedDeltaTime;
            float terrainY = terrain.SampleHeight(transform.position);
            terrainY += transform.localScale.y / 2f;
            Vector3 pos = transform.position;
            pos.y = terrainY + fixedY;
            transform.position = pos;
            Target.y = pos.y;
            yield return new WaitForFixedUpdate();
        }
        transform.position = Target;
        Destroy(gameObject);//임시
    }



    public override void Move(Vector3 direction)
    {
        terrain = Terrain.activeTerrain;
        Terrainsize = terrain.terrainData.size;
        Terrainpos = terrain.transform.position;

        Position = transform.position;

        minX = Terrainpos.x;
        maxX = Terrainpos.x + Terrainsize.x;
        minZ = Terrainpos.z;
        maxZ = Terrainpos.z + Terrainsize.z;



        Position = transform.position;
        Target = transform.position;

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

using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PewPew : EnemyBase
{

    Vector3 Position;

    float Radius;
    float MoveSpeed = 10;
    float Angle;

    int rotateDirection;   //어디로갈지 시계 반시계
    float fixedY = 0f;// 나중에 조절하게 만들꺼

    Coroutine rotateCoroutine;

    Terrain terrain;

    [SerializeField] GameObject CrashPewPew;

    Collider myCollider;

    float MoveSpeedSave;

    private PewPewSp Spawner;

    Animator animator;

    [SerializeField] float FreezeTime = 3;

    [SerializeField] GameObject IsFreeze;

    Vector3 newPos;
    Vector3 moveDir;

    float terrainY;

    [SerializeField] float CenterNoSpawn = 5f;

    public override void OnEnable()
    {
        base.OnEnable();

        animator = GetComponent<Animator>();

        if (Spawner == null)
        {
            Spawner = FindObjectOfType<PewPewSp>();
        }

        myCollider = GetComponent<Collider>();

        myCollider.enabled = false;

        terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogWarning("트레인 없다 트레인쓰세요.");
        }
        else
        {
            Vector3 tPos = terrain.transform.position;
            Vector3 tSize = terrain.terrainData.size;

            float centerX = tPos.x + tSize.x * 0.5f;
            float centerZ = tPos.z + tSize.z * 0.5f;

            Position = new Vector3(centerX, 0, centerZ);//트레인기준 중심
        }

        float RandomScale = Random.Range(1, 4) * 0.5f;
        transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale);

        Angle = Random.Range(0f, Mathf.PI * 2f);
        Radius = Random.Range(CenterNoSpawn, 20f);
        rotateDirection = Random.value < 0.5f ? 1 : -1;
        rotateCoroutine = StartCoroutine(GoPewPew());//굳이 변수 선언한건 값 초기화 때문

    }


    public override void OnDisable()
    {
        base.OnDisable();
        myCollider.enabled = false;
        ImFreeze = false;
    }


    IEnumerator GoPewPew()
    {
        myCollider = GetComponent<Collider>();

        float angularSpeed = MoveSpeed / Radius;
        Angle -= angularSpeed * Time.deltaTime * rotateDirection;


        float x = Position.x + Mathf.Cos(Angle) * Radius;
        float z = Position.z + Mathf.Sin(Angle) * Radius;

        terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;
        newPos = new Vector3(x, terrainY, z);

        moveDir = (newPos - transform.position).normalized;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
        transform.position = newPos;

        yield return new WaitForSeconds(2f);


        myCollider.enabled = true;

        while (true)
        {
            angularSpeed = MoveSpeed / Radius; //원 둘레를 도는 속도
            Angle -= angularSpeed * Time.deltaTime * rotateDirection;//각도를 회전 방향에 따라 바꿔줌

            //위치 계산해서 이동
            x = Position.x + Mathf.Cos(Angle) * Radius;
            z = Position.z + Mathf.Sin(Angle) * Radius;

            terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f + fixedY;
            newPos = new Vector3(x, terrainY, z);

            moveDir = (newPos - transform.position).normalized;

            if (moveDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
            }

            transform.position = newPos;

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Manager.Instance.observer.HitPlayer(damage);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashPewPew, hitPoint, rot);

            PoolManager.Instance.Despawn(gameObject);
            if (Spawner != null)
            {
                Spawner.SpawnOne();

            }
        }

        if (other.gameObject.tag == "Monster")
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>();
            if (otherEnemy == null)
                return; 


            if (otherEnemy.ImFreeze)
            {
                Manager.Instance.observer.HitPlayer(damage);

                Vector3 hitPoint = other.ClosestPoint(transform.position);

                Vector3 normal = (hitPoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(normal);

                GameObject inst = Instantiate(CrashPewPew, hitPoint, rot);

                PoolManager.Instance.Despawn(gameObject);
                Spawner.SpawnOne();
            }
        }
        if (other.gameObject.tag == "Aube")
        {
            Manager.Instance.observer.HitPlayer(damage);

            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Vector3 normal = (hitPoint - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(normal);

            GameObject inst = Instantiate(CrashPewPew, hitPoint, rot);

            PoolManager.Instance.Despawn(gameObject);
        }

    }



    public override void Move(Vector3 direction)
    {
        photonView.RPC("MoveRPC", RpcTarget.All, direction);
    }

    [PunRPC]
    public void MoveRPC(Vector3 direction)
    {

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
            ImFreeze = isFreeze;
            IsFreeze.SetActive(true);
            MoveSpeedSave = MoveSpeed;
            MoveSpeed = 0;
            myCollider.enabled = false;
            animator.speed = 0f;
        }
        else if (isFreeze == false)
        {
            ImFreeze = isFreeze;
            StartCoroutine(FreezeCor());
        }
        else
        {
            Debug.Log("퓨퓨 프리즈 고장남");
        }
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

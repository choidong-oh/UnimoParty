using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Burnduri : EnemyBase
{
    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>();//플레이어 여기에 등록함

    Vector3 Target;
    Vector3 myPos;

    [Header("이동 설정")]
    public float MoveSpeed = 1f;
    public float chargeSpeed = 10f;
    public float chargeDistance = 3f;

    [Header("거리 조건")]
    public float triggerDistance = 10f;
    public float fixedY = 0f;


    private bool isCharging = false;


    Terrain terrain;

    private Coroutine updateRoutine;
    private Coroutine moveRoutine;

    Transform nearestPlayer;

    int dashStateHash;




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
            Manager.Instance.observer.HitPlayer(damage);
            gameObject.SetActive(false);
        }
    }

    //void OnEnable()
    //{
    //    //한번만 찾을꺼임
    //    if (players.Count == 0)
    //    {
    //        var objs = GameObject.FindGameObjectsWithTag("Player");
    //        foreach (var obj in objs)
    //        {
    //            players.Add(obj.transform);
    //        }
    //    }

    //    terrain = Terrain.activeTerrain;
    //    isCharging = false;

    //    updateRoutine = StartCoroutine(UpdateDistance());
    //    moveRoutine = StartCoroutine(MoveRoutine());
    //}


    void OnDisable()
    {
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

        yield return new WaitForSeconds(1f);

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



        gameObject.SetActive(false);
    }





    public override void CsvEnemyInfo()
    {

    }

    public override void Move(Vector3 direction)
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

        terrain = Terrain.activeTerrain;
        isCharging = false;

        updateRoutine = StartCoroutine(UpdateDistance());
        moveRoutine = StartCoroutine(MoveRoutine());
    }

}

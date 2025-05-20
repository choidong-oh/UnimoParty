using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnduri : EnemyType
{
    [Header("플레이어 리스트")]
    public List<Transform> players = new List<Transform>(); // 인스펙터에 등록!

    [Header("이동 설정")]
    public float normalSpeed = 10f;
    public float chargeSpeed = 60f;
    public float chargeDistance = 6f;

    [Header("거리 조건")]
    public float triggerDistance = 3f;
    public float fixedY = 0f;

    private int targetIdx = -1;
    private Transform currentTarget;
    private bool isCharging = false;

    private void Awake()
    {
        if (players.Count == 0)
        {
            var objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objs)
                players.Add(obj.transform);
        }
    }

    void OnEnable()
    {

        isCharging = false;
        StartCoroutine(UpdateClosestPlayerRoutine());
        StartCoroutine(MoveRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator UpdateClosestPlayerRoutine()
    {
        while (true)
        {
            float minDist = float.MaxValue;
            int idx = -1;
            Vector3 myPos = transform.position;

            for (int i = 0; i < players.Count; i++)
            {
                var tr = players[i];
                if (tr == null) continue;

                // 여기서 오브젝트가 비활성화면 무시
                if (!tr.gameObject.activeInHierarchy) continue;

                float dist = Vector3.Distance(myPos, tr.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    idx = i;
                }
            }
            targetIdx = idx;
            currentTarget = (targetIdx >= 0 && targetIdx < players.Count) ? players[targetIdx] : null;
            yield return new WaitForSeconds(0.5f);
        }
    }


    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (currentTarget == null)
            {
                yield return null;
                continue;
            }

            if (!isCharging)
            {
                Vector3 target = currentTarget.position;
                target.y = 0;
                Vector3 pos = transform.position;
                pos.y = 0;
                Vector3 dir = (target - pos).normalized;

                if (dir != Vector3.zero)
                    transform.forward = dir;

                transform.Translate(Vector3.forward * normalSpeed * Time.deltaTime);

                // 높이 보정
                Vector3 temp = transform.position;
                if (Terrain.activeTerrain)
                    temp.y = Terrain.activeTerrain.SampleHeight(temp) + fixedY;
                else
                    temp.y = fixedY;
                transform.position = temp;

                float dist = Vector3.Distance(transform.position, currentTarget.position);
                if (dist <= triggerDistance)
                {
                    StartCoroutine(ChargeRoutine(currentTarget));
                    isCharging = true;
                    yield break; // 돌진 중엔 이동 중지
                }
            }
            yield return null;
        }
    }

    IEnumerator ChargeRoutine(Transform chargeTarget)
    {
        Vector3 start = transform.position;
        Vector3 toPlayer = (chargeTarget.position - start);
        toPlayer.y = 0;
        Vector3 dir = toPlayer.normalized;

        float moved = 0f;
        while (moved < chargeDistance)
        {
            float step = chargeSpeed * Time.deltaTime;
            transform.position += dir * step;
            moved += step;

            Vector3 temp = transform.position;
            if (Terrain.activeTerrain)
                temp.y = Terrain.activeTerrain.SampleHeight(temp) + fixedY;
            else
                temp.y = fixedY;
            transform.position = temp;

            yield return null;
        }
        gameObject.SetActive(false);
    }
}

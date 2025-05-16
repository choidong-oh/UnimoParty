using System.Collections;
using UnityEngine;

public class Burnduri : MonoBehaviour
{
    [Header("이동 설정")]
    public float normalSpeed = 10f;       // 기본 이동속도
    public float chargeSpeed = 60f;       // 돌진 시 이동속도
    public float chargeDistance = 6f;    // 돌진 거리

    [Header("거리 조건")]
    public float triggerDistance = 3f;   // 돌진 발동 거리

    public float fixedY = 0f;            // 지면 보정 높이

    private Transform player;
    private bool isCharging = false;

    void Start()
    {
        // 플레이어 찾기 (Player 태그)
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (player == null)
            {
                // 씬에 플레이어가 없으면 계속 탐색
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
                yield return null;
                continue;
            }

            if (!isCharging)
            {
                // 플레이어 방향(높이 무시)
                Vector3 target = player.position;
                target.y = 0;
                Vector3 pos = transform.position;
                pos.y = 0;
                Vector3 dir = (target - pos).normalized;

                // 바라보기
                if (dir != Vector3.zero)
                    transform.forward = dir;

                // 이동
                transform.Translate(Vector3.forward * normalSpeed * Time.deltaTime);

                // 높이 보정 (Terrain 있으면 높이 맞춤)
                Vector3 temp = transform.position;
                if (Terrain.activeTerrain)
                    temp.y = Terrain.activeTerrain.SampleHeight(temp) + fixedY;
                else
                    temp.y = fixedY;
                transform.position = temp;

                // 플레이어와의 거리 체크
                float dist = Vector3.Distance(transform.position, player.position);
                if (dist <= triggerDistance)
                {
                    // 돌진 코루틴 시작
                    StartCoroutine(ChargeRoutine());
                    isCharging = true;
                    yield break; // 이동 코루틴 중지(돌진 끝나면 비활성화)
                }
            }

            yield return null;
        }
    }

    IEnumerator ChargeRoutine()
    {
        // 돌진 방향 = 감지 순간의 플레이어 방향
        Vector3 start = transform.position;
        Vector3 toPlayer = (player.position - start);
        toPlayer.y = 0;
        Vector3 dir = toPlayer.normalized;

        float moved = 0f;
        while (moved < chargeDistance)
        {
            float step = chargeSpeed * Time.deltaTime;
            transform.position += dir * step;
            moved += step;

            // 높이 보정
            Vector3 temp = transform.position;
            if (Terrain.activeTerrain)
                temp.y = Terrain.activeTerrain.SampleHeight(temp) + fixedY;
            else
                temp.y = fixedY;
            transform.position = temp;

            yield return null;
        }

        // 돌진 끝, 오브젝트 비활성화!
        gameObject.SetActive(false);
    }
}

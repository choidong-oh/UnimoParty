using UnityEngine;
using System.Collections;

public class Pupu : MonoBehaviour
{
    // 원의 중심은 항상 (0,0,0)으로 고정
    Vector3 center = Vector3.zero;

    // Inspector에서 조절할 수 있는 값
    [Header("원 반지름(거리)")]
    public float radius = 5f;

    [Header("초기 Y높이")]
    public float fixedY = 0f;

    [Header("초당 원둘레 이동속도(m/s)")]
    public float moveSpeed = 2f;

    float angle;           // 현재 각도(라디안)
    int rotateDirection;   // 1이면 반시계, -1이면 시계

    Coroutine rotateCoroutine;

    void Start()
    {
        // 1. 랜덤 각도에서 시작
        angle = Random.Range(0f, Mathf.PI * 2f);

        // 2. 랜덤 회전 방향(1 or -1)
        rotateDirection = Random.value < 0.5f ? 1 : -1;

        // 3. 시작 위치를 원 위에 맞춰서 이동
        SetPositionOnCircle();

        // 4. 원을 따라 도는 코루틴 시작
        rotateCoroutine = StartCoroutine(RotateOnCircle());
    }

    void SetPositionOnCircle()
    {
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(center.x + x, fixedY, center.z + z);
    }

    IEnumerator RotateOnCircle()
    {
        while (true)
        {
            // 1. 원 둘레를 도는 속도 -> 각속도로 변환
            float angularSpeed = moveSpeed / radius; // 라디안/초

            // 2. 각도를 회전 방향에 따라 바꿔줌
            angle -= angularSpeed * Time.deltaTime * rotateDirection;

            // 3. 위치 계산해서 이동
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            transform.position = new Vector3(center.x + x, fixedY, center.z + z);

            yield return new WaitForFixedUpdate();
        }
    }

    // 오브젝트가 꺼질 때 코루틴 정리
    void OnDisable()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
        rotateCoroutine = null;
    }





    // (선택) 원을 Scene에 그려주는 기능
    void OnDrawGizmos()
    {
        float drawRadius = radius > 0.01f ? radius : 2.0f;
        int segments = 60;
        float theta = 0f;
        float deltaTheta = (2f * Mathf.PI) / segments;
        float y = Application.isPlaying ? transform.position.y : center.y;

        Vector3 oldPos = center + new Vector3(Mathf.Cos(0f) * drawRadius, y, Mathf.Sin(0f) * drawRadius);

        Gizmos.color = Color.white;
        for (int i = 1; i <= segments; i++)
        {
            theta += deltaTheta;
            Vector3 newPos = center + new Vector3(Mathf.Cos(theta) * drawRadius, y, Mathf.Sin(theta) * drawRadius);
            Gizmos.DrawLine(oldPos, newPos);
            oldPos = newPos;
        }
    }
}

using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class pupu : MonoBehaviour
{
    [HideInInspector] public EnemyDonutSpawner spawner;
    Vector3 center;
    float radius;
    float angle;
    int rotateDirection = 1;

    [Header("원 둘레 이동속도(m/s)")]
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float fixedY = 0.5f; // Terrain에서 띄울 높이

    void Start()
    {
        if (spawner != null)
            center = spawner.transform.position;
        else
            center = Vector3.zero;

        Vector3 dir = transform.position - center;
        radius = new Vector2(dir.x, dir.z).magnitude;
        angle = Mathf.Atan2(dir.z, dir.x);
        rotateDirection = Random.value < 0.5f ? 1 : -1;

        StartCoroutine(RotateOnCircle());
    }




    IEnumerator RotateOnCircle()
    {
        while (true)
        {
            if (radius < 0.01f) yield break;

            // 선속도 일정하게: 각속도 = 선속도 / 반지름
            float angularSpeed = moveSpeed / radius;
            angle -= angularSpeed * Time.deltaTime * rotateDirection;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // Terrain 높이 샘플링
            Vector3 targetPos = new Vector3(center.x + x, 0, center.z + z);
            float terrainY = Terrain.activeTerrain.SampleHeight(targetPos);

            // y좌표는 Terrain 높이 + 오프셋
            targetPos.y = terrainY + fixedY;

            transform.position = targetPos;

            yield return null;
        }
    }

    void OnDisable()
    {
        if (spawner != null)
            spawner.OnEnemyRemoved();
    }

    void OnDrawGizmos()
    {
        Vector3 drawCenter = spawner != null ? spawner.transform.position : transform.position;
        float drawRadius = radius > 0.01f ? radius : 2.0f;
        int segments = 60;
        float theta = 0f;
        float deltaTheta = (2f * Mathf.PI) / segments;

        // 현재 y좌표로 원 그리기(참고: 궤도는 평면에서 그림)
        float y = Application.isPlaying ? transform.position.y : drawCenter.y;

        Vector3 oldPos = drawCenter + new Vector3(Mathf.Cos(0f) * drawRadius, y, Mathf.Sin(0f) * drawRadius);

        Gizmos.color = Color.cyan;
        for (int i = 1; i <= segments; i++)
        {
            theta += deltaTheta;
            Vector3 newPos = drawCenter + new Vector3(Mathf.Cos(theta) * drawRadius, y, Mathf.Sin(theta) * drawRadius);
            Gizmos.DrawLine(oldPos, newPos);
            oldPos = newPos;
        }
    }
}

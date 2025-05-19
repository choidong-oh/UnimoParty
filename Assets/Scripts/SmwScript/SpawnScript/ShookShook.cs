using UnityEngine;
using System.Collections;

public class ShookShook : MonoBehaviour
{
    [HideInInspector] public CornerSpawner spawner;

    public float moveSpeed = 10f;
    public float fixedY = 0.5f; // 지면 높이 오프셋

    private Vector3 targetPos;

    public void Init(Vector3 startPos, Vector3 endPos)
    {
        transform.position = startPos;
        targetPos = endPos;
        StopAllCoroutines();
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget()
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.forward = dir;

        // 평면(xz) 거리로 체크
        while (Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(targetPos.x, targetPos.z)
        ) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // Terrain 높이 적용
            Vector3 temp = transform.position;
            if (Terrain.activeTerrain)
                temp.y = Terrain.activeTerrain.SampleHeight(temp) + fixedY;
            else
                temp.y = fixedY;
            transform.position = temp;

            yield return null;
        }
        // 도착점에 정확히 위치 보정
        transform.position = targetPos;
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (spawner != null)
            spawner.OnEnemyRemoved();
    }
}

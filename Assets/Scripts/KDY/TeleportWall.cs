using System.Collections;
using UnityEngine;

// 벽 방향 열거형: 어느 방향에서 순간이동할지를 정함
public enum WallType
{
    Left,
    Right,
    Top,
    Bottom
}

// 이 스크립트는 "Trigger 벽"에 붙어, 플레이어가 닿으면 순간이동시키는 역할
[RequireComponent(typeof(Collider))]
public class TeleportWall : MonoBehaviour
{
    [Header("이동 대상 위치 (필수)")]
    public Transform teleportTarget; // 이동할 위치 (안쪽 큐브 위치를 할당)

    [Header("벽의 유형 (Left / Right / Top / Bottom)")]
    public WallType wallType;

    [Header("맵 가로 길이 (X축 기준)")]
    public float mapWidth = 38f;

    [Header("맵 세로 길이 (Z축 기준)")]
    public float mapHeight = 38f;

    // 순간이동 쿨타임 설정
    private float teleportCooldown = 1f;
    private float lastTeleportTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 아닌 경우 무시
        if (!other.CompareTag("Player")) return;

        // 쿨타임 내면 무시
        if (Time.time - lastTeleportTime < teleportCooldown) return;

        // XR Origin 기준으로 처리 (최상위 Transform)
        Transform xrOrigin = other.transform.root;
        Vector3 currentPos = xrOrigin.position;
        Vector3 newPos = currentPos;

        // 벽의 방향 기준으로 이동 방향 설정 (Trigger 벽의 "전면" 방향)
        Vector3 direction = Vector3.zero;
        float distance = 0f;

        switch (wallType)
        {
            case WallType.Left:
                direction = Vector3.right;
                distance = mapWidth;
                break;
            case WallType.Right:
                direction = Vector3.left;
                distance = mapWidth;
                break;
            case WallType.Top:
                direction = Vector3.back;
                distance = mapHeight;
                break;
            case WallType.Bottom:
                direction = Vector3.forward;
                distance = mapHeight;
                break;
        }

        // 방향 * 거리만큼 이동
        newPos = currentPos + direction * distance;

        // CharacterController offset
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        float yOffset = cc != null ? cc.center.y : 0f;

        //  여기서 X축일 때 Y값 강제로 고정
        if (wallType == WallType.Left || wallType == WallType.Right)
        {
            newPos.y = 2f; // 네가 테스트해서 맞는 Y값으로 넣어
        }
        else
        {
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                float terrainY = terrain.SampleHeight(newPos) + terrain.GetPosition().y;
                newPos.y = terrainY + yOffset + 1f;
            }
        }

        // 위치 적용
        if (cc != null)
        {
            cc.enabled = true;
            Vector3 move = newPos - xrOrigin.position;
            cc.Move(move);                // 자연스러운 이동
            cc.SimpleMove(Vector3.zero); // 중력 감지 유도
        }
        else
        {
            xrOrigin.position = newPos;
        }

        // 충돌 반복 방지를 위한 쿨타임 저장
        lastTeleportTime = Time.time;

        // 디버그 출력
        Debug.Log($"{wallType} 벽 충돌: {currentPos} → {newPos} 이동 완료");
    }
}




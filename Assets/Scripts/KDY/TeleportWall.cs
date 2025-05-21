//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public enum WallType
//{
//    Left,
//    Right,
//    Top,
//    Bottom
//}

//[RequireComponent(typeof(Collider))]
//public class TeleportWall : MonoBehaviour
//{
//    public WallType wallType;
//    public float mapWidth = 38f;
//    public float mapHeight = 38f;
//    public float offset = 0.5f;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        Transform xrOrigin = other.transform.root;
//        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
//        float yOffset = cc != null ? cc.center.y : 1.0f;

//        // 현재 위치
//        Vector3 currentPos = xrOrigin.position;
//        Vector3 newPos = currentPos;

//        // 이동 방향 계산
//        Vector3 direction = Vector3.zero;
//        float distance = 0f;

//        switch (wallType)
//        {
//            case WallType.Left:
//                direction = Vector3.right;
//                distance = mapWidth;
//                break;
//            case WallType.Right:
//                direction = Vector3.left;
//                distance = mapWidth;
//                break;
//            case WallType.Top:
//                direction = Vector3.back;
//                distance = mapHeight;
//                break;
//            case WallType.Bottom:
//                direction = Vector3.forward;
//                distance = mapHeight;
//                break;

//        }

//        // 위치 계산 (기준 거리 + 여유값만큼)
//        newPos = currentPos + direction * (distance + offset);

//        // 강제로 Terrain 위로 스냅 (절대 떨어지지 않게)
//        float terrainY = GetTerrainY(newPos);
//        newPos.y = terrainY + yOffset + 0.01f;

//        // 이동 적용
//        if (cc != null)
//        {
//            cc.enabled = false;
//            xrOrigin.position = newPos;
//            cc.enabled = true;
//        }
//        else
//        {
//            xrOrigin.position = newPos;
//        }

//        // 회전 적용 (진입 반대 방향 보기)
//        Vector3 lookDir = -direction.normalized;
//        lookDir.y = 0f;
//        if (lookDir != Vector3.zero)
//        {
//            xrOrigin.rotation = Quaternion.LookRotation(lookDir);
//        }

//        Debug.Log($"[TeleportWall] {wallType} 벽 충돌 → 위치: {xrOrigin.position}");
//    }

//    // Terrain 또는 Raycast로 y값 강제 추출
//    private float GetTerrainY(Vector3 pos)
//    {
//        Terrain terrain = Terrain.activeTerrain;
//        if (terrain != null)
//        {
//            return terrain.SampleHeight(pos) + terrain.GetPosition().y;
//        }

//        // Terrain이 없다면 Raycast 시도
//        if (Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out RaycastHit hit, 10f))
//        {
//            return hit.point.y;
//        }

//        // 최후 fallback
//        return 0f;
//    }
//}

using UnityEngine;
using System.Collections;

public enum WallType
{
    Left,
    Right,
    Top,
    Bottom
}

[RequireComponent(typeof(Collider))]
public class TeleportWall : MonoBehaviour
{
    [Header("설정값")]
    public WallType wallType;

    [Tooltip("플레이어가 순간이동할 반대쪽 벽의 Transform")]
    public Transform oppositeWall;

    [Tooltip("벽 안쪽으로 들어올 여유 거리")]
    public float offset = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(TeleportRoutine(other));
    }

    private IEnumerator TeleportRoutine(Collider other)
    {
        Transform xrOrigin = other.transform.root;
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        float yOffset = cc != null ? cc.center.y : 1.0f;

        Vector3 currentPos = xrOrigin.position;
        Vector3 newPos = currentPos;

        if (oppositeWall == null)
        {
            Debug.LogWarning("[TeleportWall] oppositeWall이 인스펙터에 설정되지 않았습니다!");
            yield break;
        }

        // 반대쪽 벽의 위치 기준으로 순간이동 (X 또는 Z만 변경)
        switch (wallType)
        {
            case WallType.Left:
                newPos.x = oppositeWall.position.x - offset;
                break;
            case WallType.Right:
                newPos.x = oppositeWall.position.x + offset;
                break;
            case WallType.Top:
                newPos.z = oppositeWall.position.z - offset;
                break;
            case WallType.Bottom:
                newPos.z = oppositeWall.position.z + offset;
                break;
        }

        // Terrain 높이 보정
        float terrainY = GetTerrainY(new Vector3(newPos.x, 0, newPos.z));
        newPos.y = terrainY + yOffset + 0.01f;

        // 위치 이동
        if (cc != null)
        {
            cc.enabled = false;
            xrOrigin.position = newPos;
            yield return null;
            cc.enabled = true;
        }
        else
        {
            xrOrigin.position = newPos;
        }

        // 방향 회전 (진입 방향 반대로)
        Vector3 lookDir = (currentPos - newPos).normalized;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
        {
            xrOrigin.rotation = Quaternion.LookRotation(lookDir);
        }

        Debug.Log($"[TeleportWall] {wallType} 트리거 → {xrOrigin.position}");
    }

    private float GetTerrainY(Vector3 pos)
    {
        Terrain terrain = Terrain.activeTerrain;
        return terrain ? terrain.SampleHeight(pos) : pos.y;
    }
}


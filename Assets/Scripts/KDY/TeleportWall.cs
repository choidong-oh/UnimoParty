//using System.Collections;
//using UnityEngine;

//// 벽 방향 정의
//public enum WallType
//{
//    Left,
//    Right,
//    Top,
//    Bottom
//}

//// 이 스크립트는 플레이어가 벽에 닿았을 때,
//// 맵 반대쪽으로 순간이동시키는 역할을 한다.
//[RequireComponent(typeof(Collider))]
//public class TeleportWall : MonoBehaviour
//{
//    [Header("벽의 유형 (Left / Right / Top / Bottom)")]
//    public WallType wallType;

//    [Header("맵 가로 길이 (X축 기준)")]
//    public float mapWidth = 38f;

//    [Header("맵 세로 길이 (Z축 기준)")]
//    public float mapHeight = 38f;

//    private void OnTriggerEnter(Collider other)
//    {
//        // Player 태그가 아니면 무시
//        if (!other.CompareTag("Player")) return;

//        // XR Origin = 카메라 리그의 최상위 객체
//        Transform xrOrigin = other.transform.root;
//        Vector3 currentPos = xrOrigin.position;
//        Vector3 newPos = currentPos;

//        // 이동 방향 및 거리 설정
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

//        // 벽에서 확실히 빠져나오도록 충분히 멀리 보냄 (거리 + 2.5f 이상)
//        newPos = currentPos + direction * (distance + 0.5f);

//        // 캐릭터 컨트롤러의 중심 위치(Y축 보정용)
//        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
//        float yOffset = cc != null ? cc.center.y : 1.0f;

//        // 지형이 Terrain일 경우: 해당 위치의 높이를 샘플링
//        Terrain terrain = Terrain.activeTerrain;
//        if (terrain != null)
//        {
//            float terrainY = terrain.SampleHeight(newPos) + terrain.GetPosition().y;
//            newPos.y = terrainY + yOffset + 0.01f; // 지면 + 중심보정 + 약간 띄움
//        }
//        else
//        {
//            // Terrain이 없을 경우: Raycast로 지면 감지
//            if (Physics.Raycast(newPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
//            {
//                newPos.y = hit.point.y + yOffset + 0.01f;
//            }
//            else
//            {
//                // 최후의 fallback
//                newPos.y = yOffset + 5f;
//            }
//        }

//        // 캐릭터 이동 적용
//        if (cc != null)
//        {
//            // 충돌 오류 방지를 위해 잠시 껐다가 다시 켬
//            cc.enabled = false;
//            xrOrigin.position = newPos; // 위치 강제 설정
//            cc.enabled = true;
//        }
//        else
//        {
//            xrOrigin.position = newPos;
//        }

//        // 위치 적용이 끝난 후에 회전 적용(이게 중요함)
//        Vector3 forwardDir = direction.normalized;
//        forwardDir.y = 0f;

//        if (forwardDir != Vector3.zero)
//        {
//            //xrOrigin.rotation = Quaternion.LookRotation(forwardDir);
//            xrOrigin.rotation = Quaternion.LookRotation(-forwardDir);
//        }

//        // 순간이동 후 지면에 확실히 붙이기
//         StartCoroutine(SnapToGround(xrOrigin));

//        // 디버그 출력
//        Debug.Log($"[TeleportWall] {wallType} 벽 충돌: {currentPos} → {newPos}");
//    }

//    // 캐릭터를 지면에 강제로 붙이는 코루틴
//    IEnumerator SnapToGround(Transform target)
//    {
//        yield return new WaitForEndOfFrame();

//        // 1차 시도: Raycast로 아래 지면 감지
//        if (!Physics.Raycast(target.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
//        {
//            // 실패했을 경우 한 프레임 더 기다렸다가 재시도
//            yield return new WaitForEndOfFrame();

//            if (Physics.Raycast(target.position + Vector3.up * 2f, Vector3.down, out hit, 10f))
//            {
//                Vector3 pos = target.position;
//                pos.y = hit.point.y;
//                target.position = pos;
//            }
//        }
//        else
//        {
//            // 성공했으면 바로 지면 위치로 맞춤
//            Vector3 pos = target.position;
//            pos.y = hit.point.y;
//            target.position = pos;
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public WallType wallType;
    public float mapWidth = 38f;
    public float mapHeight = 38f;
    public float offset = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Transform xrOrigin = other.transform.root;
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        float yOffset = cc != null ? cc.center.y : 1.0f;

        // 현재 위치
        Vector3 currentPos = xrOrigin.position;
        Vector3 newPos = currentPos;

        // 이동 방향 계산
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

        // 위치 계산 (기준 거리 + 여유값만큼)
        newPos = currentPos + direction * (distance + offset);

        // 강제로 Terrain 위로 스냅 (절대 떨어지지 않게)
        float terrainY = GetTerrainY(newPos);
        newPos.y = terrainY + yOffset + 0.01f;

        // 이동 적용
        if (cc != null)
        {
            cc.enabled = false;
            xrOrigin.position = newPos;
            cc.enabled = true;
        }
        else
        {
            xrOrigin.position = newPos;
        }

        // 회전 적용 (진입 반대 방향 보기)
        Vector3 lookDir = -direction.normalized;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
        {
            xrOrigin.rotation = Quaternion.LookRotation(lookDir);
        }

        Debug.Log($"[TeleportWall] {wallType} 벽 충돌 → 위치: {xrOrigin.position}");
    }

    // Terrain 또는 Raycast로 y값 강제 추출
    private float GetTerrainY(Vector3 pos)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            return terrain.SampleHeight(pos) + terrain.GetPosition().y;
        }

        // Terrain이 없다면 Raycast 시도
        if (Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out RaycastHit hit, 10f))
        {
            return hit.point.y;
        }

        // 최후 fallback
        return 0f;
    }
}





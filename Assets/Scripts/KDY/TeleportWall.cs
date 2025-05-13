//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// TeleportWall: 플레이어가 벽에 닿았을 때 반대편으로 순간이동시키는 스크립트
//// 벽의 유형(WallType)에 따라 이동 방향이 결정됨
//// 맵 크기(mapWidth, mapHeight)는 플레이어가 얼마나 멀리 순간이동할지 결정함
//// XR Origin 기반 플레이어에도 대응하며, Terrain 위 착지를 위해 Raycast 처리 포함

//public enum WallType
//{
//    Left,    // 왼쪽 벽
//    Right,   // 오른쪽 벽
//    Top,     // 위쪽 벽
//    Bottom   // 아래쪽 벽
//}

//public class TeleportWall : MonoBehaviour
//{
//    [Header("벽의 유형 (Left / Right / Top / Bottom)")]
//    public WallType wallType; // 현재 벽의 방향 지정

//    [Header("맵 가로 길이 (X축 기준)")]
//    public float mapWidth = 38f; // X축 기준 순간이동 거리

//    [Header("맵 세로 길이 (Z축 기준)")]
//    public float mapHeight = 38f; // Z축 기준 순간이동 거리

//    private float teleportCooldown = 0.5f; // 쿨타임 설정 (초)
//    private float lastTeleportTime = -999f; // 마지막 순간이동 시간 저장

//    private void OnTriggerEnter(Collider other)
//    {
//        // "Player" 태그가 아닌 경우 무시
//        if (!other.CompareTag("Player")) return;

//        // 쿨타임 내 중복 감지 방지
//        if (Time.time - lastTeleportTime < teleportCooldown) return;

//        // 플레이어 오브젝트의 최상위 루트 (XR Origin 전체) 기준으로 이동 처리
//        Transform xrOrigin = other.transform.root;

//        // 현재 위치 저장
//        Vector3 currentPos = xrOrigin.position;
//        Vector3 newPos = currentPos;

//        // 벽의 방향에 따라 반대편 위치 계산
//        switch (wallType)
//        {
//            case WallType.Left:
//                newPos = new Vector3(currentPos.x + mapWidth, currentPos.y, currentPos.z);
//                break;
//            case WallType.Right:
//                newPos = new Vector3(currentPos.x - mapWidth, currentPos.y, currentPos.z);
//                break;
//            case WallType.Top:
//                newPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - mapHeight);
//                break;
//            case WallType.Bottom:
//                newPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + mapHeight);
//                break;
//        }

//        // 위에서 아래로 Ray를 쏴서 Terrain 위 Y좌표 확인
//        Vector3 rayStart = newPos + Vector3.up * 10f;
//        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f))
//        {
//            // 지형 위로 정확히 착지
//            newPos.y = hit.point.y;
//        }

//        // XR Origin 위치 이동
//        //xrOrigin.position = newPos;

//        // CharacterController 있을 경우 충돌 방지를 위해 일시적으로 끔
//        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
//        if (cc != null)
//            cc.enabled = false;

//        // 위치 이동
//        xrOrigin.position = newPos;

//        // 다시 켜기
//        if (cc != null)
//            cc.enabled = true;


//        // 회전 초기화 (X/Z 회전 꼬임 방지)
//        xrOrigin.rotation = Quaternion.identity;

//        // 쿨타임 시간 저장
//        lastTeleportTime = Time.time;

//        // 디버그 출력
//        Debug.Log($"{wallType} 벽 충돌: {currentPos} → {newPos} 이동 완료 (XR Origin 회전 초기화됨)");

//    }
//}

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

        // 회전 초기화 (필요시 제거 가능)
        //xrOrigin.rotation = Quaternion.identity;

        // 충돌 반복 방지를 위한 쿨타임 저장
        lastTeleportTime = Time.time;

        // 순간적으로 Trigger 충돌 꺼두기
       // StartCoroutine(TemporarilyDisableCollider(other));

        // 디버그 출력
        Debug.Log($"{wallType} 벽 충돌: {currentPos} → {newPos} 이동 완료");
    }

    //// 충돌 반복 방지용: 잠시 Collider 비활성화
    //private IEnumerator TemporarilyDisableCollider(Collider col)
    //{
    //    col.enabled = false;
    //    yield return new WaitForSeconds(teleportCooldown);
    //    col.enabled = true;
    //}
    
}




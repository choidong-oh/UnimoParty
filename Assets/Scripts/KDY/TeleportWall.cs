using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TeleportWall: 플레이어가 벽에 부딪히면 반대편으로 이동시키는 스크립트
// 벽 유형별로 Left, Right, Top, Bottom 구분하여 설정
// mapWidth와 mapHeight는 맵 전체 크기 (Plane 스케일 x 10 기준)로 설정

public enum WallType
{
    Left,
    Right,
    Top,
    Bottom
}

public class TeleportWall : MonoBehaviour
{
    [Header("벽의 유형 (Left / Right / Top / Bottom)")]
    public WallType wallType;

    [Header("맵 가로 길이 (전체 X축 크기, 예: 400)")]
    public float mapWidth = 400f;

    [Header("맵 세로 길이 (전체 Z축 크기, 예: 400)")]
    public float mapHeight = 400f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 currentPos = other.transform.position;
        Vector3 newPos = currentPos;

        switch (wallType)
        {
            case WallType.Left:
                // 왼쪽 벽 → 오른쪽으로 이동 (X 반대편)
                newPos = new Vector3(currentPos.x + mapWidth, currentPos.y, currentPos.z);
                break;
            case WallType.Right:
                // 오른쪽 벽 → 왼쪽으로 이동
                newPos = new Vector3(currentPos.x - mapWidth, currentPos.y, currentPos.z);
                break;
            case WallType.Top:
                // 위쪽 벽 → 아래쪽으로 이동 (Z 반대편)
                newPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - mapHeight);
                break;
            case WallType.Bottom:
                // 아래쪽 벽 → 위쪽으로 이동
                newPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + mapHeight);
                break;
        }

        other.transform.position = newPos;

        // 기존에는 회전을 유지하거나 Y축만 보정하는 방식을 사용했으나,
        // 이번에는 항상 초기화(0,0,0 회전)하도록 변경하여 테스트함.
        other.transform.rotation = Quaternion.identity;

        Debug.Log($"{wallType} 벽 충돌: {currentPos} → {newPos} 이동 완료 (회전 초기화됨)");
    }
}


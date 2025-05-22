using UnityEngine;

public class WallTeleporter : MonoBehaviour
{
    public enum WallType { Top, Bottom, Left, Right }

    [Header("벽의 위치 방향을 선택")]
    public WallType wallType;

    [Header("반대편 벽 위치를 오브")]
    public Transform teleportReference; // 반대편 기준점

    [Header("LookAt 대상 (플레이어가 바라보게 할 오브젝트)")]
    public Transform lookAtTarget;


    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지고 있지 않으면 무시
        if (!other.CompareTag("Player")) return;

        // 플레이어 Transform 정보 가져오기
        Transform player = other.transform;

        // 현재 위치 저장
        Vector3 currentPos = player.position;

        // 새로운 위치는 현재 위치를 기본으로 하되, 일부 축만 변경함
        Vector3 newPos = currentPos;

        // 벽 방향에 따라 반대편의 해당 축 좌표로 덮어쓰기
        switch (wallType)
        {
            case WallType.Top:    // 위쪽 벽: Z축만 이동
                newPos.z = teleportReference.position.z;
                break;

            case WallType.Bottom: // 아래쪽 벽: Z축만 이동
                newPos.z = teleportReference.position.z;
                break;

            case WallType.Left:   // 왼쪽 벽: X축만 이동
                newPos.x = teleportReference.position.x;
                break;

            case WallType.Right:  // 오른쪽 벽: X축만 이동
                newPos.x = teleportReference.position.x;
                break;
        }

        // 최종 위치 적용
        player.position = newPos;


        // LookAt 대상이 할당되어 있다면, 바라보도록 처리
        if (lookAtTarget != null)
        {
            player.LookAt(lookAtTarget);
        }
    }


}

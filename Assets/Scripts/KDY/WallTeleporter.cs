using UnityEngine;

public class WallTeleporter : MonoBehaviour
{
    public enum WallType { Top, Bottom, Left, Right }
    public WallType wallType;

    public Transform teleportReference; // 반대편 기준점 (Empty Object)

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Transform player = other.transform;
        Vector3 currentPos = player.position;
        Vector3 newPos = currentPos; // 복사해서 수정

        // 벽 기준 축 이동
        switch (wallType)
        {
            case WallType.Top:    // +Z → -Z
                newPos.z = teleportReference.position.z;
                break;
            case WallType.Bottom: // -Z → +Z
                newPos.z = teleportReference.position.z;
                break;
            case WallType.Left:   // -X → +X
                newPos.x = teleportReference.position.x;
                break;
            case WallType.Right:  // +X → -X
                newPos.x = teleportReference.position.x;
                break;
        }

        // 위치 이동
        player.position = newPos;

        // 로컬 방향 판단 (벽 기준에서 플레이어 forward가 뒤면 회전)
        Vector3 localForward = transform.InverseTransformDirection(player.forward);
        if (localForward.z < 0f)
        {
            Vector3 euler = player.eulerAngles;
            euler.y += 180f;
            player.rotation = Quaternion.Euler(euler);
        }
    }
}

using Ilumisoft.RadarSystem;
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

        //if (!other.CompareTag("Player")) return;  몬스터
        //if (!other.CompareTag("Player")) return;  몬스터
        
        // 플레이어 Transform 정보 가져오기
        Transform player = other.transform;
        
        //Transform player = other.transform;  몬스터
        //Transform player = other.transform;  몬스터

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


        // LookAt 대상이 설정되어 있는 경우에만 회전 처리 실행
        if (lookAtTarget != null)
        {
            // LookAt 대상과 플레이어 사이의 방향 벡터 계산
            // 단, 수직 방향(Y축)은 무시하여 XZ 평면 상에서의 방향만 사용
            Vector3 direction = lookAtTarget.position - player.position;
            direction.y = 0f; // Y축을 0으로 만들어 기울어진 회전(Pitch, Roll) 제거

            // 방향 벡터가 너무 작으면 회전할 필요가 없으므로 무시
            if (direction.sqrMagnitude > 0.001f)
            {
                // 대상 방향을 바라보기 위한 회전 값(Quaternion) 계산
                Quaternion targetRot = Quaternion.LookRotation(direction);

                // 현재 플레이어의 회전값을 Euler(각도) 형태로 가져옴
                Vector3 currentEuler = player.eulerAngles;

                // 목표 회전값을 Euler 각도로 변환
                Vector3 targetEuler = targetRot.eulerAngles;

                // 최종 회전 적용: 현재 X(고개 기울기)와 Z(몸 기울기)는 그대로 유지하고,
                // Y(좌우 회전)만 목표 방향으로 덮어씀
                player.rotation = Quaternion.Euler(currentEuler.x, targetEuler.y, currentEuler.z);

                // 레이더 회전 즉시 갱신
                player.GetComponentInChildren<Radar>().RefreshRotationImmediately();
            }
        }
    }

    // 변경 가능 할 수 있음 
    //private void OnTriggerExit(Collider other)
    //{
    //    // 충돌한 객체가 "Player" 태그를 가지고 있지 않으면 무시
    //    if (!other.CompareTag("Player")) return;
    //    //if (!other.CompareTag("Player")) return;  몬스터 태그로 변경
    //    //if (!other.CompareTag("Player")) return;  몬스터 태그로 변경

    //    // 플레이어 Transform 정보 가져오기
    //    Transform player = other.transform;
    //    //Transform player = other.transform;    몬스터로 변경
    //    //Transform player = other.transform;    몬스터로 변경
         
    //    // 현재 위치 저장//
    //    Vector3 currentPos = player.position;

    //    // 새로운 위치는 현재 위치를 기본으로 하되, 일부 축만 변경함
    //    Vector3 newPos = currentPos;

    //    // 벽 방향에 따라 반대편의 해당 축 좌표로 덮어쓰기
    //    switch (wallType)
    //    {
    //        case WallType.Top:    // 위쪽 벽: Z축만 이동
    //            newPos.z = teleportReference.position.z;
    //            break;

    //        case WallType.Bottom: // 아래쪽 벽: Z축만 이동
    //            newPos.z = teleportReference.position.z;
    //            break;

    //        case WallType.Left:   // 왼쪽 벽: X축만 이동
    //            newPos.x = teleportReference.position.x;
    //            break;

    //        case WallType.Right:  // 오른쪽 벽: X축만 이동
    //            newPos.x = teleportReference.position.x;
    //            break;
    //    }

    //    // 최종 위치 적용
    //    player.position = newPos;
    //}
}

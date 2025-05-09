using UnityEngine;

public class RotateSetting : MonoBehaviour
{
    public Transform hmdTransform;         // HMD 카메라
    public Transform spaceShip;            // 기준점 & 조이스틱 포함 오브젝트
    public float thresholdAngle = 60f;     // 트리거 각도
    public float rotateSpeed = 60f;        

    private bool isRotating = false;
    private int rotateDirection = 0;
    private Vector3 baseForward;

    void Start()
    {
        baseForward = FlatDirection(spaceShip.forward); // 기준점은 spaceShip의 정면
    }

    void Update()
    {
        Vector3 currentHMDForward = FlatDirection(hmdTransform.forward);
        float angle = Vector3.SignedAngle(baseForward, currentHMDForward, Vector3.up);

        if (!isRotating)
        {
            if (angle > thresholdAngle)
            {
                rotateDirection = 1;
                isRotating = true;
            }
            else if (angle < -thresholdAngle)
            {
                rotateDirection = -1;
                isRotating = true;
            }
        }
        else
        {
            // HMD가 계속 회전 범위 밖에 있는 동안 회전 유지
            if ((rotateDirection == 1 && angle > thresholdAngle) ||
                (rotateDirection == -1 && angle < -thresholdAngle))
            {
                float rotationAmount = rotateSpeed * Time.deltaTime * rotateDirection;

                // 기준점과 spaceShip을 같이 회전
                transform.Rotate(0f, rotationAmount, 0f);
                spaceShip.Rotate(0f, rotationAmount, 0f);
            }
            else
            {
                // HMD가 다시 기준 범위 안에 들어옴 → 회전 종료 + 기준 갱신
                isRotating = false;
                baseForward = FlatDirection(spaceShip.forward); // 새 기준점은 현재 spaceShip의 정면
            }
        }
    }

    Vector3 FlatDirection(Vector3 dir)
    {
        dir.y = 0f;
        return dir.normalized;
    }
}
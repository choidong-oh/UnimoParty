using UnityEngine;

public class RotateSetting : MonoBehaviour
{
    public Transform cameraTF;          // HMD 카메라
    public Transform spaceShip;         // 기준이 되는 오브젝트
    public float rotateSpeed = 60f;     // 초당 회전 속도 (deg/sec)

    private bool isRotating = false;
    private int rotateDirection = 0;

    void Update()
    {
        Vector3 forward = FlatDirection(spaceShip.forward);
        Vector3 camDir = FlatDirection(cameraTF.forward);
        float angle = Vector3.SignedAngle(forward, camDir, Vector3.up);

        if (!isRotating)
        {
            if (angle > 60f)
            {
                rotateDirection = 1;
                isRotating = true;
                Debug.Log("오른쪽 회전 시작");
            }
            else if (angle < -60f)
            {
                rotateDirection = -1;
                isRotating = true;
                Debug.Log("왼쪽 회전 시작");
            }
        }
        else
        {
            // 범위 밖이면 계속 회전
            bool stillOutside =
                (rotateDirection == 1 && angle > 60f) ||
                (rotateDirection == -1 && angle < -60f);

            if (stillOutside)
            {
                float rotAmount = rotateSpeed * Time.deltaTime * rotateDirection;

                transform.Rotate(0f, rotAmount, 0f);
                spaceShip.Rotate(0f, rotAmount, 0f);
                cameraTF.Rotate(0f, rotAmount, 0f); 
            }
            else
            {
                isRotating = false;
                Debug.Log("회전 멈춤");
            }
        }
    }

    Vector3 FlatDirection(Vector3 dir)
    {
        dir.y = 0f;
        return dir.normalized;
    }
}

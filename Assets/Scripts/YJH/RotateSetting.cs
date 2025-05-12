using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class RotateSetting : MonoBehaviour
{
    public Transform cameraTF;
    public Transform spaceShip;
    public float rotateSpeed = 60f;

    private bool isRotating = false;
    private int rotateDirection = 0;
    private float baseYAngle;

    void Start()
    {
        // 기준은 항상 spaceShip의 정면 Y 회전
        baseYAngle = spaceShip.eulerAngles.y;
    }

    void Update()
    {
        float currentYAngle = cameraTF.eulerAngles.y;

        // 기준 방향(spaceShip)과 HMD 방향의 Y축 회전 차이 계산 (-180~180)
        float deltaAngle = Mathf.DeltaAngle(baseYAngle, currentYAngle);

        if (!isRotating)
        {
            if (deltaAngle >= 60f)
            {
                Debug.Log("오른쪽 회전 시작");
                rotateDirection = 1;
                isRotating = true;
            }
            else if (deltaAngle <= -60f)
            {
                Debug.Log("왼쪽 회전 시작");
                rotateDirection = -1;
                isRotating = true;
            }
        }
        else
        {
            // 계속 회전 유지 조건 (계속 기준 바깥에 있을 때만)
            if ((rotateDirection == 1 && deltaAngle >= 60f) ||
                (rotateDirection == -1 && deltaAngle <= -60f))
            {
                float rotationAmount = rotateSpeed * Time.deltaTime * rotateDirection;
                transform.Rotate(0f, rotationAmount, 0f);
                spaceShip.Rotate(0f, rotationAmount, 0f);
            }
            else
            {
                Debug.Log("회전 멈춤 + 기준 갱신");
                isRotating = false;

                // 기준을 현재 spaceShip 방향으로 갱신
                baseYAngle = spaceShip.eulerAngles.y;
            }
        }
    }

    //public Transform cameraTF;
    //public Transform spaceShip;            
    //public float rotateSpeed = 60f;        

    //private bool isRotating = false;
    //private int rotateDirection = 0;
    //private Vector3 baseForward;

    //void Start()
    //{
    //    baseForward = FlatDirection(spaceShip.forward); // 기준점은 spaceShip의 정면
    //}

    //void Update()
    //{
    //    float angle = cameraTF.eulerAngles.y;

    //    // 기준 회전 각도를 baseForward 기준이 아니라 절대값 기준으로 처리
    //    float relativeAngle = Mathf.DeltaAngle(baseForward.y, angle); // -180~180

    //    if (!isRotating)
    //    {
    //        if (relativeAngle >= 60f)
    //        {
    //            Debug.Log("오른쪽 회전 시작");
    //            rotateDirection = 1;
    //            isRotating = true;
    //        }
    //        else if (relativeAngle <= -60f)
    //        {
    //            Debug.Log("왼쪽 회전 시작");
    //            rotateDirection = -1;
    //            isRotating = true;
    //        }
    //    }
    //    else
    //    {
    //        if ((rotateDirection == 1 && relativeAngle >= 60f) || (rotateDirection == -1 && relativeAngle <= -60f))
    //        {
    //            float rotationAmount = rotateSpeed * Time.deltaTime * rotateDirection;
    //            transform.Rotate(0f, rotationAmount, 0f);
    //            spaceShip.Rotate(0f, rotationAmount, 0f);
    //        }
    //        else
    //        {
    //            Debug.Log("회전 멈춤");
    //            isRotating = false;
    //            baseForward = spaceShip.eulerAngles; // 새로운 기준 Y각도 저장
    //        }
    //    }
    //}

    //Vector3 FlatDirection(Vector3 dir)
    //{
    //    Debug.Log("정면 셋팅");
    //    dir.y = 0f;
    //    return dir.normalized;
    //}
}
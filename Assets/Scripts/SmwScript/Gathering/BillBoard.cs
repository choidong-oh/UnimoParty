using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        // 카메라를 바라보되, 오브젝트는 항상 정면을 유지하게 회전
        Vector3 camPos = targetCamera.transform.position;
        transform.LookAt(transform.position + (transform.position - camPos));
    }
}

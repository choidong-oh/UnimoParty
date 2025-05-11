using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeadDash : MonoBehaviour
{
    [Header("XR Origin")]
    public Transform xrOrigin; // XR Origin을 인스펙터에서 할당

    [Header("Dash Settings")]
    public float dashAngle = 30f;           // X회전 트리거 각도
    public float dashDistance = 2f;         // 대쉬 거리
    public float dashCooldown = 5f;         // 쿨타임 (초)
    public float dashDuration = 0.5f;       // 대쉬 지속 시간 (초)

    [Header("Dash Events")]
    public UnityEvent onDashForward;
    public UnityEvent onDashBackward;
    public UnityEvent onDashReady;
    public UnityEvent onDashBlocked;

    private float lastDashTime = -999f;     // 마지막 대쉬 시간
    private bool isDashing = false;         // 중복 방지

    void Update()
    {
        if (isDashing || xrOrigin == null)
            return;

        float xRot = transform.eulerAngles.x;

        // 360도 → -180 ~ 180 범위로 보정
        if (xRot > 180f) xRot -= 360f;

        // 회전 각도 조건 충족 시
        if (Mathf.Abs(xRot) >= dashAngle)
        {
            if (Time.time - lastDashTime >= dashCooldown)
            {
                onDashReady?.Invoke();

                if (xRot >= dashAngle)
                {
                    StartCoroutine(SmoothDash(xrOrigin.forward));
                    onDashForward?.Invoke();
                }
                else if (xRot <= -dashAngle)
                {
                    StartCoroutine(SmoothDash(-xrOrigin.forward));
                    onDashBackward?.Invoke();
                }

                lastDashTime = Time.time;
            }
            else
            {
                onDashBlocked?.Invoke();
            }
        }
    }

   

    public void DashForward()
    {
        if (!isDashing && xrOrigin != null)
        {
            Debug.Log("앞으로 대쉬");
            StartCoroutine(SmoothDash(xrOrigin.forward));
            lastDashTime = Time.time;
        }
    }

    public void DashBackward()
    {
        if (!isDashing && xrOrigin != null)
        {
            Debug.Log("뒤로 대쉬");
            StartCoroutine(SmoothDash(-xrOrigin.forward));
            lastDashTime = Time.time;
        }
    }

    private IEnumerator SmoothDash(Vector3 direction)
    {
        isDashing = true;

        Vector3 startPos = xrOrigin.position;
        Vector3 endPos = startPos + direction.normalized * dashDistance;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            xrOrigin.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        xrOrigin.position = endPos; // 위치 보정
        isDashing = false;
    }
}

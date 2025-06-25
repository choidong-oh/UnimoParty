using Photon.Pun;
using System.Collections;
using UnityEngine;

public class HeadDash : MonoBehaviourPunCallbacks, IFreeze
{
    [Header("XR Origin")]
    public Transform xrOrigin;            // XR Origin 오브젝트
    public Transform spaceShip;           // 방향 기준

    [Header("Dash Settings")]
    public float dashAngle = 30f;         // 회전 트리거 각도 (X/Z)
    public float dashDistance = 2f;       // 대시 거리
    public float dashCooldown = 5f;       // 쿨타임
    public float dashDuration = 0.5f;     // 대시 지속 시간

    public DashCooldown dashCooldownUI; 

    private float lastDashTime = -999f;
    public bool isDashing = false;

    //freeze 변수
    float tempDashDistance = 1F;
    float tempDashCooldown = 5f;
    private void Start()
    {
        tempDashDistance = dashDistance;
        tempDashCooldown = dashCooldown;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            float xRot = transform.eulerAngles.x;
            float zRot = transform.eulerAngles.z;

            // 오일러 각도 보정 (-180 ~ 180)
            if (xRot > 180f) xRot -= 360f;
            if (zRot > 180f) zRot -= 360f;

            bool canDash = Time.time - lastDashTime >= dashCooldown;

            // X축: 앞뒤 대시
            if (canDash)
            {
                if (xRot >= dashAngle)
                {
                    StartCoroutine(SmoothDash(spaceShip.forward));
                    lastDashTime = Time.time;
                }
                else if (xRot <= -dashAngle)
                {
                    StartCoroutine(SmoothDash(-spaceShip.forward));
                    lastDashTime = Time.time;
                }
                else if (zRot >= dashAngle)
                {
                    StartCoroutine(SmoothDash(-spaceShip.right));
                    lastDashTime = Time.time;
                }
                else if (zRot <= -dashAngle)
                {
                    StartCoroutine(SmoothDash(spaceShip.right));
                    lastDashTime = Time.time;
                }
            }
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

        xrOrigin.position = endPos;
        isDashing = false;

        dashCooldownUI.StartCooldown(); //  UI 연동
    }

    //어름
    public void Freeze(bool IsFreeze)
    {
        if (IsFreeze == true)
        {
            dashDistance = 0;
            dashCooldown = 0;
        }
        else if (IsFreeze == false)
        {
            dashDistance = tempDashDistance;
            dashCooldown = tempDashCooldown;
        }

    }
}

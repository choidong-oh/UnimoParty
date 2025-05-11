using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;


public class LeftHandController : MonoBehaviour
{
    public GameObject leftHand;

    public XRJoystick joystick;
    public GameObject xrOriginObject;     // XROrigin 오브젝트
    public float moveSpeed = 1.0f;        // 이동 속도
    public Transform stickPos;

    private Transform xrOriginTransform;

    bool select= false;
    void Start()
    {
        xrOriginTransform = xrOriginObject.transform;

        joystick.onValueChangeY.AddListener(OnJoystickMoveY);
        joystick.onValueChangeX.AddListener(OnJoystickMoveX);
    }
    void OnDestroy()
    {
        joystick.onValueChangeY.RemoveListener(OnJoystickMoveY);
        joystick.onValueChangeX.RemoveListener(OnJoystickMoveX);
    }

    private void OnJoystickMoveY(float value)
    {
        Vector3 forward = new Vector3(xrOriginTransform.forward.x, 0f, xrOriginTransform.forward.z).normalized;
        xrOriginTransform.position += forward * value * moveSpeed * Time.deltaTime;
    }

    private void OnJoystickMoveX(float value)
    {
        Vector3 right = new Vector3(xrOriginTransform.right.x, 0f, xrOriginTransform.right.z).normalized;
        xrOriginTransform.position += right * value * moveSpeed * Time.deltaTime;
    }

    public void OnSelectEnter()
    {
        Debug.Log("+");
        select = true;
        StartCoroutine(FreezeHand());
    }
    public void OnSelectExit()
    {
        Debug.Log("-");
        select = false;
    }

    private IEnumerator FreezeHand()
    {
        while (select)
        {
            yield return new WaitForEndOfFrame(); // XR이 다 끝나고 나서 강제로 적용
            leftHand.transform.position = stickPos.position;
            leftHand.transform.rotation = stickPos.rotation;
        }
    }
}

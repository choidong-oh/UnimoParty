using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftHandController : MonoBehaviour
{
    public ActionBasedController leftController;

    public GameObject leftHand;
    public GameObject controllerPrefab;
    public XRJoystick joystick;
    public Transform joystickTF;
    public GameObject xrOriginObject;     // XROrigin 오브젝트
    public float moveSpeed = 1.0f;        // 이동 속도

    private Transform xrOriginTransform;

    GameObject LC;
    void Start()
    {
        LC = Instantiate(controllerPrefab, joystickTF);        
        LC.SetActive(false);

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
        leftController.modelPrefab = null;
        LC.SetActive(true);
    }
    public void OnSelectExit()
    {
        leftController.modelPrefab = controllerPrefab.transform;
        LC.SetActive(false);
    }
}

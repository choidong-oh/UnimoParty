using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestGrab : MonoBehaviour
{

    private XRGrabInteractable grabInteractable;
    [SerializeField] Transform player;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }


    void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("±×·¦ÇÜ");

        grabInteractable.attachTransform = player;
        Transform attach = grabInteractable.attachTransform;
        Vector3 currentRotation = attach.localEulerAngles;
        attach.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y + 90f, currentRotation.z);

    }

        void OnRelease(SelectExitEventArgs args)
    {

    }

}

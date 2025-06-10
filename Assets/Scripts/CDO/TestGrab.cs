using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestGrab : MonoBehaviour
{

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }


    void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("±×·¦ÇÜ");

        var rotation = gameObject.transform.eulerAngles;
        rotation.y += 90;
        transform.Rotate(rotation);


    }

    void OnRelease(SelectExitEventArgs args)
    {

    }

}

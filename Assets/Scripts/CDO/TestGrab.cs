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

        Vector3 pos = gameObject.transform.position;
        pos.x = 0;
        pos.y += 90;
        pos.z = 0;

        transform.Rotate(pos);


    }

    void OnRelease(SelectExitEventArgs args)
    {

    }

}

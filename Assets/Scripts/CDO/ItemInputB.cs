using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


//secondaryButton[오른손 XR 컨트롤러] = B 버튼
//primaryButton[오른손 XR 컨트롤러] = A 버튼
public class ItemInputB : MonoBehaviourPunCallbacks, IFreeze
{
    [SerializeField] InputActionReference BInputActionReference; //xr b

    [SerializeField] InputActionReference triggerInputActionReference; //트리거

    Transform firepos; //모델의 끝부분

    [SerializeField] Transform rightController; //오른쪽 컨트롤러

    GameObject Item1 = null;

    int grenadePower = 5;



    public override void OnEnable()
    {
        base.OnEnable();
        BInputActionReference.action.Enable();
        BInputActionReference.action.performed += ControllerB;

        triggerInputActionReference.action.performed += OnTriggerPressed;
        triggerInputActionReference.action.canceled += OnTriggerReleased;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        BInputActionReference.action.performed -= ControllerB;
        BInputActionReference.action.Disable();

        triggerInputActionReference.action.performed -= OnTriggerPressed;
        triggerInputActionReference.action.canceled -= OnTriggerReleased;
    }

    //트리거 함
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        //photonView.RPC("Item1Rpc",RpcTarget.All);
        //아이템 사용
      
        Rigidbody rb = Item1.gameObject.GetComponent<Rigidbody>();
        Vector3 throwDirection = firepos.transform.forward + firepos.transform.up;
        Item1.transform.parent = null;
        rb.useGravity = true;
        rb.AddForce(throwDirection * grenadePower, ForceMode.VelocityChange);

    }

   


    //트리거 뗌
    void OnTriggerReleased(InputAction.CallbackContext context)
    {



    }
    public void StateItem(bool isItemInputB)
    {
        if (isItemInputB == true)
        {
            firepos = rightController.gameObject.GetComponentInChildren<ActionBasedController>().model.GetChild(0).transform;
            Item1 = PhotonNetwork.Instantiate("Boomprefab", firepos.position, Quaternion.identity);
            Item1.transform.parent = firepos.transform;
            Item1.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        else if(isItemInputB == false)
        {
            PhotonNetwork.Destroy(Item1);
        }
    }

    //아이템 교체 b
    private void ControllerB(InputAction.CallbackContext context)
    {
        Debug.Log("컨트롤 b버튼, 아이템 교체");
    }

    public void Freeze(bool IsFreeze)
    {
        throw new System.NotImplementedException();
    }
}

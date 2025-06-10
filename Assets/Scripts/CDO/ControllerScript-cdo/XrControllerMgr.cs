using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XrControllerMgr : MonoBehaviourPunCallbacks
{
    [SerializeField] //아이템 > 채집총 교체 a
    InputActionReference AInputActionReference;

    //게임오브젝트 활성화 비활성화용
    [SerializeField] GameObject handHarvestObj;
    [SerializeField] GameObject ItemObj;

    [SerializeField] ItemInputB itemInputB;

    [SerializeField] GameObject ItemGun;
    [SerializeField] GameObject harvestGun;
    [SerializeField] ActionBasedController rightController;

    GameObject RController;

    bool isItemController = false;   //처음은 채집총 시작

    private void Start()
    {
        RController = Instantiate(ItemGun, rightController.gameObject.transform);
        RController.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        AInputActionReference.action.Enable();
        AInputActionReference.action.performed += ControllerA;

    }

    public override void OnDisable()
    {
        base.OnDisable();
        AInputActionReference.action.performed -= ControllerA;
        AInputActionReference.action.Disable();

    }

    Transform lasetRcontroller;
    //아이템, 채집총 교체 a
    private void ControllerA(InputAction.CallbackContext context)
    {
        if (photonView.IsMine)
        {

            if (isItemController == false)
            {
                Debug.Log("컨트롤 a버튼 채집총 > 아이템총 교체");

                isItemController = true;
                IsItemObj(true);
                itemInputB.StateItem(true);
            }
            else if (isItemController == true)
            {
                Debug.Log("컨트롤 a버튼 아이템총 > 채집총 교체");

                isItemController = false;
                itemInputB.StateItem(false);
                IsItemObj(false);
            }
        }

    }

    void IsItemObj(bool isItem)
    {
        ItemObj.SetActive(isItem);
        handHarvestObj.SetActive(!isItem);
        if(isItem == true)
        {
            lasetRcontroller = rightController.model;
            rightController.model.gameObject.SetActive(false);
            RController.SetActive(true);
            rightController.model = RController.transform;
            rightController.modelPrefab = RController.transform;
        }
        else if(isItem == false)
        {
            rightController.model = lasetRcontroller;
            rightController.model.gameObject.SetActive(true);
            RController.SetActive(false);
            rightController.model = lasetRcontroller;
            rightController.modelPrefab = lasetRcontroller;

        }

    }


   

    



}

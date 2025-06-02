using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XrControllerMgr : MonoBehaviour
{
    [SerializeField] //아이템 > 채집총 교체
    InputActionReference AInputActionReference; 

    //게임오브젝트 활성화 비활성화용
    [SerializeField] GameObject HandHarvestObj;
    [SerializeField] GameObject ItemObj;

    bool isItemController = false;   //처음은 채집총 시작

    private void OnEnable()
    {
        AInputActionReference.action.Enable();
        AInputActionReference.action.performed += ControllerA;

    }

    private void OnDisable()
    {
        AInputActionReference.action.performed -= ControllerA;
        AInputActionReference.action.Disable();

    }


    //아이템, 채집총 교체 a
    private void ControllerA(InputAction.CallbackContext context)
    {
        if (isItemController == true)
        {
            Debug.Log("컨트롤 a버튼 채집총 > 아이템총 교체");
            IsItemObj(true);
        }
        else if (isItemController == false)
        {
            Debug.Log("컨트롤 a버튼 아이템총 > 채집총 교체");
            IsItemObj(false);
        }

    }

    void IsItemObj(bool isItem)
    {
        ItemObj.SetActive(isItem);
        HandHarvestObj.SetActive(!isItem);
    }






}

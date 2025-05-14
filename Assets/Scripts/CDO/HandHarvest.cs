using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHarvest : MonoBehaviour
{
    [SerializeField] XRRayInteractor RayInteractor;

    [SerializeField]int SpiritPoint = 0; //
    public int spiritPoint {  get { return SpiritPoint; } set { if (value < 0) { Debug.Log("정령음수됨");value = 0; } SpiritPoint = value; }}

    [SerializeField] private InputActionReference activateAction;
    Flower flower;

    //콜백은 OnEnable 안댐
    //player은 안사라지니깐 awake, start에 넣으면 댈듯 
    //콜백 쓸만한건없긴함
    void OnEnable()
    {
        activateAction.action.performed += OnTriggerPressed;
        activateAction.action.canceled += OnTriggerReleased;
    }

    void OnDisable()
    {
        activateAction.action.performed -= OnTriggerPressed;
        activateAction.action.canceled -= OnTriggerReleased;
    }

    //안전코드 써야댐 flower가없을수있음
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        flower = null;

        if (RayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {

            if (!hit.collider.TryGetComponent<Flower>(out flower))
            {
                return;
            }

            //더 안전한 코드
            if (flower != null)
            {
                flower.StartHarvest();
                Debug.Log("Flower 수확 시작!");
            }
        }
    }

    void OnTriggerReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger 뗌");
        if (flower.gameObject.activeSelf == true && flower !=null)
        {
            flower.StopHarvest();
        }
    }

    //정령 전달
    public int DeliverySpirit()
    {
        int SpiritReturnPoint = spiritPoint;
        spiritPoint = 0;
        return SpiritReturnPoint;
    }


}

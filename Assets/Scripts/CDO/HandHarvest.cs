using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHarvest : MonoBehaviourPunCallbacks
{
    [SerializeField] XRRayInteractor RayInteractor;

    [SerializeField] int SpiritPoint = 0; //
    public int spiritPoint { get { return SpiritPoint; } set { if (value < 0) { Debug.Log("정령음수됨"); value = 0; } SpiritPoint = value; } }

    [SerializeField] private InputActionReference activateAction;
    Flower flower;

    [Header("Haptic 진동 관련")]
    [SerializeField] float HapticAmplitude;
    [SerializeField] float HapticDuraiton;

    //콜백은 OnEnable 안댐
    //player은 안사라지니깐 awake, start에 넣으면 댈듯 
    //콜백 쓸만한건없긴함
    public override void OnEnable()
    {
        base.OnEnable();
        if (photonView.IsMine)
        {            
            activateAction.action.performed += OnTriggerPressed;
            activateAction.action.canceled += OnTriggerReleased;
        }        
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (photonView.IsMine)
        {
            activateAction.action.performed -= OnTriggerPressed;
            activateAction.action.canceled -= OnTriggerReleased;
        }
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
                //진동
                //*인스펙터창에서 다른진동 Haptic 0으로 줄여야댐
                //그래야 애만 진동됌
                RayInteractor.xrController.SendHapticImpulse(HapticAmplitude, HapticDuraiton);

                flower.Init(this);
                flower.StartHarvest();
                Debug.Log("Flower 수확 시작!");
            }
        }
    }

    void OnTriggerReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger 뗌");
        if (flower != null && flower.gameObject.activeSelf == true)
        {
            flower.StopHarvest();
        }
    }


    //정령 전달
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Manager.Instance.observer.DeliveryFairy();
        }
    }

}

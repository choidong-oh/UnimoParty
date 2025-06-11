using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHarvest : MonoBehaviourPunCallbacks, IFreeze
{
    [SerializeField] GameObject cameraOffset;

    [SerializeField] XRRayInteractor rayInteractor;

    [SerializeField] int spiritPoint = 0; //
    public int SpiritPoint { get { return spiritPoint; } set { if (value < 0) { Debug.Log("정령음수됨"); value = 0; } spiritPoint = value; } }

    [SerializeField] private InputActionReference activateAction;
    Flower flower;

    [Header("Haptic 진동 관련")]
    [SerializeField] float hapticAmplitude;
    [SerializeField] float hapticDuraiton;
    int gap = 10;

    //콜백은 OnEnable 안댐
    //player은 안사라지니깐 awake, start에 넣으면 댈듯 
    //콜백 쓸만한건없긴함
    private void Start()
    {
        if (!photonView.IsMine)
        {
            cameraOffset.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(FlowerDistanceCor());
        }
    }

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

    public void Freeze(bool isFreeze)
    {
        Debug.Log("HandHarvest의 Freeze실행됌");
        if (isFreeze)
        {
            activateAction.action.performed -= OnTriggerPressed;
            activateAction.action.canceled -= OnTriggerReleased;
        }
        else if (!isFreeze)
        {
            activateAction.action.performed += OnTriggerPressed;
            activateAction.action.canceled += OnTriggerReleased;
        }

    }


    //안전코드 써야댐 flower가없을수있음
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        flower = null;
        IsHarvest(false);

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
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
                //rayInteractor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuraiton);

                IsHarvest(true);

                flower.Init(this);
                flower.StartHarvest();
                Debug.Log("Flower 수확 시작!");




            }
        }
    }
    Coroutine cor;
    public void IsHarvest(bool isHarvest)
    {
        if (isHarvest)
        {
            cor = StartCoroutine(IsHarvestCor());
            //rayInteractor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuraiton);
        }
        else if (!isHarvest)
        {
            if (cor != null)
            {
                StopCoroutine(cor);
            }
            rayInteractor.xrController.SendHapticImpulse(0, 0);
        }

    }

    IEnumerator IsHarvestCor()
    {
        while (true)
        {
            rayInteractor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuraiton);
            yield return new WaitForSeconds(hapticDuraiton);
        }

    }



    private void Update()
    {
        if (flower != null)
        {
            if (Vector3.Distance(flower.gameObject.transform.position, rayInteractor.transform.position) >= 5)
            {
                if (flower != null && flower.gameObject.activeSelf == true)
                {
                    IsHarvest(false);
                    flower.StopHarvest();

                }
               
            }

            if (flower.gameObject.activeSelf == false)
            {
                IsHarvest(false);

            }

        }
    }


    IEnumerator FlowerDistanceCor()
    {
        while (true)
        {
            if (flower != null)
            {
                if (Vector3.Distance(flower.gameObject.transform.position, rayInteractor.transform.position) >= 5)
                {
                    if (flower != null && flower.gameObject.activeSelf == true)
                    {
                        IsHarvest(false);
                        flower.StopHarvest();

                    }

                }

                if (flower.gameObject.activeSelf == false)
                {
                    IsHarvest(false);

                }

            }


            yield return null;
        }
    }


    void OnTriggerReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger 뗌");
        if (flower != null && flower.gameObject.activeSelf == true)
        {
            IsHarvest(false);
            flower.StopHarvest();
            flower = null;
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

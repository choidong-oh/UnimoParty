using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHarvest : MonoBehaviourPunCallbacks, IFreeze
{
    [SerializeField] GameObject cameraOffset;

    [SerializeField] XRRayInteractor rayInteractor;

    [SerializeField] XrControllerMgr xrControllerMgr;

    [SerializeField] Transform rightController; //오른쪽 컨트롤러

    [SerializeField] int spiritPoint = 0; //
    public int SpiritPoint { get { return spiritPoint; } set { if (value < 0) { Debug.Log("정령음수됨"); value = 0; } spiritPoint = value; } }

    [SerializeField] private InputActionReference activateAction;
    Flower flower;

    [Header("Haptic 진동 관련")]
    [SerializeField] float hapticAmplitude;
    [SerializeField] float hapticDuraiton;
    int gap = 10;

    public Queue<string> itemQueue = new Queue<string>();

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
        StartCoroutine(wait111());
    }

    IEnumerator wait111()
    {
        yield return new WaitUntil(() => rightController.gameObject.activeSelf);
        OnEnableItem();

    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (photonView.IsMine)
        {
            activateAction.action.performed += OnTriggerPressed;
            activateAction.action.canceled += OnTriggerReleased;
        }

        StartCoroutine(waitr());
        //itemQueue = xrControllerMgr.publicitemQueue;
    }

    IEnumerator waitr()
    {
        yield return new WaitForSeconds(1);
        itemQueue = xrControllerMgr.publicitemQueue;
        InvenCreateItem();
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

    public void AddQueueItem(string ItemName)
    {
        itemQueue.Enqueue(ItemName);
        InvenCreateItem();
    }

    //아이템 위치 (자식객체)
    Transform invenItem1;
    Transform invenItem2;
    Transform invenItem3;

    //아이템 생성 프리팹
    GameObject item1;
    GameObject item2;
    GameObject item3;

    void OnEnableItem()
    {
        var modelGun1 = rightController.gameObject.GetComponent<ActionBasedController>();

        var modelGun2 = modelGun1.model;

        invenItem1 = modelGun2.GetChild(2);
        invenItem2 = modelGun2.GetChild(3);
        invenItem3 = modelGun2.GetChild(4);

    }

    void InvenCreateItem()
    {

        if (item1 != null)
        {
            PhotonNetwork.Destroy(item1);
        }
        if (item2 != null)
        {
            PhotonNetwork.Destroy(item2);
        }
        if (item3 != null)
        {
            PhotonNetwork.Destroy(item3);
        }




        string[] arr = itemQueue.ToArray();
        if (arr.Length == 0)
        {
            return;
        }

        item1 = PhotonNetwork.Instantiate(arr[0], transform.position, Quaternion.identity);
        item1.transform.SetParent(invenItem1, false);
        item1.transform.localPosition = Vector3.zero;
        item1.GetComponent<Rigidbody>().useGravity = false;
        item1.transform.localScale = Vector3.one * 0.5f;

        if (arr.Length == 1)
        {
            return;
        }

        item2 = PhotonNetwork.Instantiate(arr[1], transform.position, Quaternion.identity);
        item2.transform.SetParent(invenItem2, false);
        item2.transform.localPosition = Vector3.zero;
        item2.GetComponent<Rigidbody>().useGravity = false;
        item2.transform.localScale = Vector3.one * 0.5f;

        if (arr.Length == 2)
        {
            return;
        }

        item3 = PhotonNetwork.Instantiate(arr[2], transform.position, Quaternion.identity);
        item3.transform.SetParent(invenItem3, false);
        item3.transform.localPosition = Vector3.zero;
        item3.GetComponent<Rigidbody>().useGravity = false;
        item3.transform.localScale = Vector3.one * 0.5f;



    }


}

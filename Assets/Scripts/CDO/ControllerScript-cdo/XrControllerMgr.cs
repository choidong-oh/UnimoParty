using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XrControllerMgr : MonoBehaviourPunCallbacks
{
    //아이템총 인풋액션(인풋 프라이머리)
    [SerializeField] InputActionReference AInputActionReference;
    [SerializeField] ActionBasedController rightController;

    //게임오브젝트 활성화 비활성화용
    [Header("각 스크립트 들어가있는 오브젝트")]
    [SerializeField] GameObject handHarvestObj;
    [SerializeField] GameObject ItemObj;
    [SerializeField] ItemInputB itemInputB;
    [SerializeField] HandHarvest handHarvest;

    [Header("총 프리팹 (모델)")]
    [SerializeField] GameObject harvestGun;
    [SerializeField] GameObject ItemGun;


    bool isItemController = false;   //처음은 채집총 시작
    GameObject RController;
    Transform lasetRcontroller; //temp

    //총 가지고 있는 아이템
    public Queue<string> publicitemQueue = new Queue<string>();


    #region 종현삭제금지
    //종현이꺼
    [SerializeField] InputActionReference MenuInputActionReference;
    [SerializeField] GameObject optionPanel;
    private bool isOption = false;
    #endregion

    private void Start()
    {
        RController = Instantiate(ItemGun, rightController.gameObject.transform);
        RController.SetActive(false);


        optionPanel.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        AInputActionReference.action.Enable();
        AInputActionReference.action.performed += ControllerA;


        MenuInputActionReference.action.Enable();
        MenuInputActionReference.action.performed += ControllerMenu;

        //임시 아이템 추가
        ItemQueueAdd("start");

    }

    public override void OnDisable()
    {
        base.OnDisable();
        AInputActionReference.action.performed -= ControllerA;
        AInputActionReference.action.Disable();

        MenuInputActionReference.action.performed -= ControllerMenu;
        MenuInputActionReference.action.Disable();

    }
    private void ControllerMenu(InputAction.CallbackContext context)
    {
        if (photonView.IsMine)
        {
            isOption = !isOption;
            if (isOption)
            {
                optionPanel.SetActive(true);
            }
            else
            {
                optionPanel.SetActive(false);
            }
        }
    }

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

    public void IsItemObj(bool isItem)
    {
        ItemQueueAdd("start");
        ItemObj.SetActive(isItem);
        handHarvestObj.SetActive(!isItem);
        if (isItem == true)
        {
            lasetRcontroller = rightController.model;
            rightController.model.gameObject.SetActive(false);
            RController.SetActive(true);
            rightController.model = RController.transform;
            rightController.modelPrefab = RController.transform;
        }
        else if (isItem == false)
        {
            rightController.model = lasetRcontroller;
            rightController.model.gameObject.SetActive(true);
            RController.SetActive(false);
            rightController.model = lasetRcontroller;
            rightController.modelPrefab = lasetRcontroller;

        }


    }

    void ItemQueueAdd(string item)
    {
        if (publicitemQueue.Count == 0)
        {
            publicitemQueue.Enqueue("Boomprefab");
            publicitemQueue.Enqueue("PotionPrefab2");
            publicitemQueue.Enqueue("TestItem1");
        }

        if(item == "start")
        {
            return;
        }

        publicitemQueue.Enqueue(item);
        var newItem = publicitemQueue.Peek();
        itemInputB.AddQueueItem(newItem);
        handHarvest.AddQueueItem(newItem);
    }
  




}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


//1. 아이템들이있음 //아이템1, 아이템2, 아이템3 
//2. 아이템들(프리팹이름만 담고 사용한다)
//3. 큐에 아이템들을 담음 
//4. 현재 아이템 = 큐에 첫번째
//5. 아이템의기능 아이템의use함수를 사용
//6. 아이템 사용함수는 컨트롤러의 함수 사용
//7. 교체는 첫번째아이템을 다시 큐에담음


//secondaryButton[오른손 XR 컨트롤러] = B 버튼
//primaryButton[오른손 XR 컨트롤러] = A 버튼
public class ItemInputB : MonoBehaviourPunCallbacks, IFreeze
{
    [SerializeField] InputActionReference bInputActionReference; //xr b

    [SerializeField] InputActionReference triggerInputActionReference; //트리거

    Transform firepos; //모델의 끝부분

    [SerializeField] Transform rightController; //오른쪽 컨트롤러

    int grenadePower = 2;

    public Queue<string> itemQueue = new Queue<string>();

    GameObject currentItem = null;
    GameObject newItem;

    //스크립트
    [SerializeField] XrControllerMgr xrControllerMgr;


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
        var modelGun2 = rightController.gameObject.GetComponent<ActionBasedController>().model;


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

    public override void OnEnable()
    {
        base.OnEnable();
        bInputActionReference.action.Enable();
        bInputActionReference.action.performed += ControllerB;

        triggerInputActionReference.action.performed += OnTriggerPressed;


        //아이템 입힘
        itemQueue = xrControllerMgr.publicitemQueue;

    }

    public override void OnDisable()
    {
        base.OnDisable();
        bInputActionReference.action.performed -= ControllerB;
        bInputActionReference.action.Disable();

        triggerInputActionReference.action.performed -= OnTriggerPressed;
    }

    //트리거 함
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        //아이템 사용
        if (currentItem.TryGetComponent<IItemUse>(out var itemUse))
        {
            if (itemUse.Use(firepos, grenadePower) == false)
            {
                return;
            }

        }



        //아이템사용하면 큐 삭제
        itemQueue.Dequeue();
        //큐에 담겨진 아이템이 없으면
        if (itemQueue.Count <= 0)
        {
            xrControllerMgr.IsItemObj(false);
            return;
        }
        string nextItem = itemQueue.Peek();
        currentItem = ItemCreate(nextItem);
    }



    GameObject ItemCreate(string ItemPrefabName)
    {
        if(ItemPrefabName == "TestItem1")
        {
            ItemPrefabName = "TestItem";
        }
        firepos = rightController.gameObject.GetComponentInChildren<ActionBasedController>().model.GetChild(0).transform;
        newItem = PhotonNetwork.Instantiate(ItemPrefabName, firepos.position, Quaternion.identity);
        newItem.transform.rotation = rightController.transform.rotation;
        newItem.transform.parent = firepos;
        newItem.gameObject.GetComponent<Rigidbody>().useGravity = false;

        //OnEnableInvenItem();
        InvenCreateItem();
        return newItem;
    }

    //프리팹 생성
    public void StateItem(bool isItemInputB)
    {
        if (isItemInputB == true)
        {
            OnEnableItem();
            //if (itemQueue.Count == 0)
            //{
            //    itemQueue.Enqueue("Boomprefab");
            //    itemQueue.Enqueue("PotionPrefab1");
            //    itemQueue.Enqueue("TestItem");
            //}
            if (currentItem == null)
            {
                string QueueItem = itemQueue.Peek();
                currentItem = ItemCreate(QueueItem);
            }

        }
        else if (isItemInputB == false)
        {
            if (currentItem != null)
            {
                PhotonNetwork.Destroy(currentItem);
                currentItem = null;
            }
        }
    }

    //아이템 교체 b
    private void ControllerB(InputAction.CallbackContext context)
    {
        //혹시 몰라 안전코드
        if (itemQueue.Count <= 0)
        {
            xrControllerMgr.IsItemObj(false);
            return;
        }

        Debug.Log("컨트롤 b버튼, 아이템 교체");
        string oldItem = itemQueue.Dequeue();
        itemQueue.Enqueue(oldItem);

        PhotonNetwork.Destroy(currentItem);

        if (itemQueue.Count <= 0)
        {
            xrControllerMgr.IsItemObj(false);
            return;
        }
        string nextItem = itemQueue.Peek();
        currentItem = ItemCreate(nextItem);


    }

    public void IsItemQueueCountZero()
    {
        //큐에 담겨진 아이템이 없으면
        if (itemQueue.Count <= 0)
        {
            xrControllerMgr.IsItemObj(false);
            return;
        }
    }

    public void AddQueueItem(string ItemName)
    {
        itemQueue.Enqueue(ItemName);
        InvenCreateItem();
    }

    public void Freeze(bool IsFreeze)
    {
        throw new System.NotImplementedException();
    }
}

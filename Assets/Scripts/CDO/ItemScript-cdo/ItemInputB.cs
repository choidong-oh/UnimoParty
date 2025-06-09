using Photon.Pun;
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

    public override void OnEnable()
    {
        base.OnEnable();
        bInputActionReference.action.Enable();
        bInputActionReference.action.performed += ControllerB;

        triggerInputActionReference.action.performed += OnTriggerPressed;
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
            itemUse.Use(firepos, grenadePower);
        }

        //아이템사용하면 큐 삭제
        itemQueue.Dequeue();
        string nextItem = itemQueue.Peek();
        currentItem = ItemCreate(nextItem);

    }

    GameObject ItemCreate(string ItemPrefabName)
    {
        firepos = rightController.gameObject.GetComponentInChildren<ActionBasedController>().model.GetChild(0).transform;
        newItem = PhotonNetwork.Instantiate(ItemPrefabName, firepos.position, Quaternion.identity);
        newItem.transform.parent = firepos;
        newItem.gameObject.GetComponent<Rigidbody>().useGravity = false;

        return newItem;
    }

    //프리팹 생성
    public void StateItem(bool isItemInputB)
    {
        if (isItemInputB == true)
        {
            if (itemQueue.Count == 0)
            {
                itemQueue.Enqueue("Boomprefab");
                itemQueue.Enqueue("PotionPrefab1");
            }
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
        if (itemQueue.Count <= 0)
        {
            return;
        }
        Debug.Log("컨트롤 b버튼, 아이템 교체");
        Debug.Log("itemQueue.count = " + itemQueue.Count);
        string oldItem = itemQueue.Dequeue();
        itemQueue.Enqueue(oldItem);

        PhotonNetwork.Destroy(currentItem);

        if (itemQueue.Count <= 0)
        {
            return;
        }
        Debug.Log("큐에들어갈프리팹이름 = " + itemQueue.Peek());
        string nextItem = itemQueue.Peek();
        currentItem = ItemCreate(nextItem);


    }

    public void AddQueueItem(string ItemName)
    {
        itemQueue.Enqueue(ItemName);
    }

    public void Freeze(bool IsFreeze)
    {
        throw new System.NotImplementedException();
    }
}

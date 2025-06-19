using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Gun1ItemTest : MonoBehaviour
{
    [SerializeField] Transform rightController; //오른쪽 컨트롤러
    //인벤 위치
    GameObject itemInven1;
    GameObject itemInven2;
    GameObject itemInven3;

    //인벤 아이템
    GameObject invenItem1;
    GameObject invenItem2;
    GameObject invenItem3;


    private void OnEnable()
    {
        InvenItemStart();

    }

    //처음 인벤 아이템 start호출
    void InvenItemStart()
    {
        var controller = rightController.GetComponent<ActionBasedController>().model;
        itemInven1 = controller.transform.GetChild(3).gameObject;
        itemInven2 = controller.transform.GetChild(4).gameObject;
        itemInven3 = controller.transform.GetChild(5).gameObject;
    }

    //void InvenCreate()
    //{
    //    string[] arr = itemQueue.ToArray();
    //    if (arr.Length == 0)
    //    {
    //        return;
    //    }
    //    invenItem1 = PhotonNetwork.Instantiate(arr[0], Vector3.zero, Quaternion.identity);
    //    invenItem1.transform.parent = itemInven1.transform;
    //    invenItem1.GetComponent<Rigidbody>().useGravity = false;
    //    invenItem1.transform.localScale = Vector3.one * 0.5f;
    //    invenItem1.transform.localPosition = Vector3.zero;
    //    if (arr.Length == 1)
    //    {
    //        return;
    //    }

    //    invenItem2 = PhotonNetwork.Instantiate(arr[1], Vector3.zero, Quaternion.identity);
    //    invenItem2.transform.parent = itemInven2.transform;
    //    invenItem2.GetComponent<Rigidbody>().useGravity = false;
    //    invenItem2.transform.localScale = Vector3.one * 0.5f;
    //    invenItem2.transform.localPosition = Vector3.zero;
    //    if (arr.Length == 2)
    //    {
    //        return;
    //    }
    //    invenItem3 = PhotonNetwork.Instantiate(arr[2], Vector3.zero, Quaternion.identity);
    //    invenItem3.transform.parent = itemInven3.transform;
    //    invenItem3.GetComponent<Rigidbody>().useGravity = false;
    //    invenItem3.transform.localScale = Vector3.one * 0.5f;
    //    invenItem3.transform.localPosition = Vector3.zero;


    //}






}

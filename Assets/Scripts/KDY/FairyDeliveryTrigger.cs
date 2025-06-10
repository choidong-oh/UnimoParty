using UnityEngine;

public class FairyDeliveryTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(" 플레이어가 AUBE에 닿음 페어리 반납");
            Manager.Instance.observer.DeliveryFairy();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(" 플레이어가 AUBEASDASD에 닿음 페어리 반납");
            Manager.Instance.observer.DeliveryFairy();
        }
    }

}

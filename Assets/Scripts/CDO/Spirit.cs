using UnityEngine;

public partial class Spirit : MonoBehaviour
{
    public int SpiritPointRepository;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "Player")
        {
            Debug.Log("콜라이더1");
            if (collision.gameObject.TryGetComponent<HandHarvest>(out HandHarvest player))
            {
                Debug.Log("콜라이더2");
                SpiritPointRepository += player.DeliverySpirit();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}

public partial class Spirit : MonoBehaviour
{
        





}

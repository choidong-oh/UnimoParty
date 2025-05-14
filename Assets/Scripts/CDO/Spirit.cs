using UnityEngine;

public class Spirit : MonoBehaviour
{
    public int SpiritPointRepository;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == "Player")
        {
            Debug.Log("콜라이더");
            SpiritPointRepository++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }


}

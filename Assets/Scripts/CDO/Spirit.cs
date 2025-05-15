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
    //private int tempFairyValue_1 = 10;
    //private int tempFairyValue_2 = 10;
    //private int tempFairyValue_3 = 10;

    // 채집물을 반납을 할 때, 
    // 아래 경로로 통해 메소드를 불러오면 됌;
    private void CallMethod()
    {
        Manager.Instance.observer.DeliveryFairy();
    }

    private void Start()
    {
        SetDefaultCount();
    }

    void SetDefaultCount()
    {
        Manager.Instance.goalCount.GoalFairyValue_1 = Manager.Instance.tempFairyValue_1;
        Manager.Instance.goalCount.GoalFairyValue_2 = Manager.Instance.tempFairyValue_2;
        Manager.Instance.goalCount.GoalFairyValue_3 = Manager.Instance.tempFairyValue_3;
    }
}

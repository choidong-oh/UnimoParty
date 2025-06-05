using UnityEngine;

public class Fragment : MonoBehaviour
{
    [SerializeField] Bungpeo Parents;
    [SerializeField] GameObject CrashBunpeoFragment;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Instantiate(CrashBunpeoFragment, transform.position, Quaternion.identity);
        }
        if (collision.gameObject.tag == "Player")
        {
            Instantiate(CrashBunpeoFragment, transform.position, Quaternion.identity);
        }
        Parents.IsActivate();//나중엔 이걸로 써야함
        //Parents.IsActivateRPC();//이거는 테스트용
        gameObject.SetActive(false);
    }

}

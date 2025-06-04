using System.Collections;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    int damage = 1;

    float ExplosionRadius = 1;
    float FirstRadius;

    SphereCollider myCollider;



    [SerializeField] GameObject CrashBunpeoFragment;

    private void Start()
    {
        myCollider = GetComponent<SphereCollider>();
        FirstRadius = myCollider.radius;
    }

    private void OnDisable()
    {
        myCollider.radius = FirstRadius;


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {

            Instantiate(CrashBunpeoFragment, transform.position, Quaternion.identity);
        }
        if (collision.gameObject.tag == "Player")
        {
            Instantiate(CrashBunpeoFragment, transform.position, Quaternion.identity);
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

        }
    }



}

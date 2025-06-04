using System.Collections;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    int damage = 1;

    float ExplosionRadius = 1;
    float FirstRadius;

    SphereCollider myCollider;

    GameObject SkinMash;

    [SerializeField] GameObject CrashBunpeoFragment;

    private void Start()
    {
        myCollider = GetComponent<SphereCollider>();
        FirstRadius = myCollider.radius;
    }

    private void OnDisable()
    {
        myCollider.radius = FirstRadius;
        SkinMash.SetActive(true);
        gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            StartCoroutine(GroundChack());
            Instantiate(CrashBunpeoFragment, transform.position, Quaternion.identity);
        }
        if (collision.gameObject.tag == "Player")
        {
            Instantiate(CrashBunpeoFragment, transform.position, Quaternion.identity);
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);
            StartCoroutine(GroundChack());
        }
    }
    IEnumerator GroundChack()
    {
        SkinMash.SetActive(false);
        myCollider.radius = ExplosionRadius;
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }


}

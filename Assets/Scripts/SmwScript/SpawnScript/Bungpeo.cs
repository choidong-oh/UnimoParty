using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bungpeo : MonoBehaviour
{
    public float explosionForce = 500f;      
    public float explosionRadius = 5f;    
    public float upwardsModifier = 1f;       //위로 틔어오름
    public LayerMask explosionMask;

    public GameObject[] Fragment;

    public GameObject explosionFragment;

    public GameObject Body;

    Animator animator;          
    string inflateStateName = "anim_MON003_ready01";    // 부풀어오르는 애니메이션 State 이름
    string explodeStateName = "anim_MON003_ready02"; // 마지막 폭발 전 애니메이션 State 이름

    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
        animator.Play(inflateStateName);
        StartCoroutine(WaitAndExplode());
        //Explode();
    }

    private IEnumerator WaitAndExplode()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        yield return null;
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float inflateLength = stateInfo.length;
        yield return new WaitForSeconds(inflateLength);
        animator.Play(explodeStateName);

        yield return null;
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float explodeAnimLength = stateInfo.length;
        yield return new WaitForSeconds(explodeAnimLength);

        Explode();
    }


    private void OnDisable()
    {
        foreach (GameObject fragment in Fragment)
        {
            Collider col = fragment.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;  
                rb.useGravity = false;    
            }
            fragment.transform.position = Vector3.zero;
        }

    }

    public void Explode()
    {

        Vector3 explosionPosition = transform.position;

        foreach (GameObject fragment in Fragment)
        {
            Collider col = fragment.GetComponent<Collider>();
            if (col != null)
                col.enabled = true;

            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;  
                rb.useGravity = true;    
            }
        }


        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius, explosionMask);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }

        GameObject inst = Instantiate(explosionFragment, transform.position, Quaternion.identity);

        //Body.SetActive(false);
    }

}

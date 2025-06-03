using System.Collections;
using UnityEngine;

public class Bungpeo : MonoBehaviour
{
    public float explosionForce = 20f;
    public float explosionRadius = 20f;
    public float upwardsModifier = 1f;
    public LayerMask explosionMask;

    public GameObject[] Fragment;

    public GameObject explosionFragment;

    public GameObject[] Body;

    Animator animator;

    Collider myCollider;

    private string explodeStateName = "anim_MON003_ready02";

    private Vector3 initialScale;
    private void Awake()
    {
        initialScale = transform.localScale;
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        // ▶ 변경된 부분: Animator가 키프레임 스케일을 먼저 적용하지 못하도록 비활성화
        animator.enabled = false;

        // ▶ 변경된 부분: 오브젝트 스케일을 항상 초기 상태(initialScale)로 강제 복원
        transform.localScale = initialScale;

        // ▶ 변경된 부분: Animator 내부 상태를 초기화
        animator.Rebind();
        animator.Update(0f);

        // ▶ 변경된 부분: 준비가 끝났으면 Animator를 다시 활성화
        animator.enabled = true;



        myCollider.enabled = true;



        for (int i = 0; i < Fragment.Length; i++)
        {

            if (Fragment[i].GetComponent<Collider>() != null)
            {
                Fragment[i].GetComponent<Collider>().enabled = false;
            }

            if (Fragment[i].GetComponent<Rigidbody>() != null)
            {
                Fragment[i].GetComponent<Rigidbody>().isKinematic = true;
                Fragment[i].GetComponent<Rigidbody>().useGravity = false;
            }
            Fragment[i].transform.localPosition = Vector3.zero;
        }


        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(true);
            myCollider.enabled = true;
        }

        StartCoroutine(WaitAndExplode());
    }

    private IEnumerator WaitAndExplode()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(explodeStateName))
        {
            yield return null;
        }

        // 2) 해당 상태(State)가 재생되는 순간, 연결된 클립 정보(대개 하나)에서 길이(length) 가져오기
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length == 0)
        {
            Debug.LogWarning($"[{explodeStateName}] 상태에 연결된 애니메이션 클립이 없습니다.");
            yield break;
        }

        float clipLength = clips[0].clip.length;

        yield return new WaitForSeconds(clipLength);

        Explode();
    }


    private void OnDisable()
    {
        for (int i = 0; i < Fragment.Length; i++)
        {

            if (Fragment[i].GetComponent<Collider>() != null)
            {
                Fragment[i].GetComponent<Collider>().enabled = false;
            }

            if (Fragment[i].GetComponent<Rigidbody>() != null)
            {
                Fragment[i].GetComponent<Rigidbody>().isKinematic = true;
                Fragment[i].GetComponent<Rigidbody>().useGravity = false;
            }
            Fragment[i].transform.localPosition = Vector3.zero;
        }
        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(true);
        }
    }

    public void Explode()
    {

        Vector3 explosionPosition = transform.position;

        for (int i = 0; i < Fragment.Length; i++)
        {

            if (Fragment[i].GetComponent<Collider>() != null)
            {
                Fragment[i].GetComponent<Collider>().enabled = true;
            }

            if (Fragment[i].GetComponent<Rigidbody>() != null)
            {
                Fragment[i].GetComponent<Rigidbody>().isKinematic = false;
                Fragment[i].GetComponent<Rigidbody>().useGravity = true;
            }
            Fragment[i].transform.localPosition = Vector3.zero;
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

        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(false);
            myCollider.enabled = false;
        }


    }

}

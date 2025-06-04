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
        // 1) 애니메이터를 잠시 끈다
        animator.enabled = false;

        // 2) transform 스케일을 초기값으로 강제 복원
        transform.localScale = initialScale;

        // 3) 애니메이터 내부 상태 완전 초기화
        animator.Rebind();
        animator.Update(0f);

        // 4) 진입 상태(Entry)애니메이션을 0초 지점으로 강제 재생
        //    애니메이터 컨트롤러에서 해당 레이어의 기본 스테이트 이름을 정확히 넣어주자.
        string entryStateName = "Idle"; // 예시: Entry 상태 이름(Animator Controller에 설정된 첫 스테이트)
        animator.Play(entryStateName, 0, 0f);
        animator.Update(0f); // ★여기서 다시 한 번 0초 프레임을 적용해서 transform.localScale=1,1,1이 확실히 반영되도록 함

        // 5) 이제 애니메이터를 켜면, 설정한 첫 프레임 상태(스케일=1,1,1)에서부터 재생된다
        animator.enabled = true;





        myCollider.enabled = true;


        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(true);
        }

        StartCoroutine(WaitAndExplode());
    }

    private IEnumerator WaitAndExplode()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(explodeStateName))
        {
            yield return null;
        }

        // 해당 상태(State)가 재생되는 순간, 연결된 클립 정보(대개 하나)에서 길이(length) 가져오기
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

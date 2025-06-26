//범퍼
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bungpeo : EnemyBase
{

    [SerializeField] float explosionForce = 20f;
    [SerializeField] float explosionRadius = 20f;
    [SerializeField] float upwardsModifier = 1f;
    [SerializeField] LayerMask explosionMask;

    [SerializeField] GameObject[] Fragment;

    [SerializeField] GameObject explosionPartycle;

    [SerializeField] GameObject[] Body;

    Animator animator;

    Collider myCollider;
    string AppearAni = "anim_MON003_appear";
    string explodeStateName = "anim_MON003_ready02";

    [SerializeField] GameObject CrashBunpeo;

    Terrain terrain;

    int IsActivateFragment = 0;

    [SerializeField] GameObject IsFreeze;

    [SerializeField] float FreezeTime = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.CompareTag("Player"))
        {
            if (ImFreeze == true)
            {
                ImFreeze = false;
                StartCoroutine(FreezeCor());
            }
            else if (ImFreeze == false)
            {
                damage = 1;
                Manager.Instance.observer.HitPlayer(damage);

                Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게
                Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
                Quaternion rot = Quaternion.LookRotation(normal);// 방향계산
                Instantiate(CrashBunpeo, hitPoint, rot);


                PoolManager.Instance.DespawnNetworked(gameObject);

            }
        }

        else if (other.CompareTag("Monster"))
        {
            EnemyBase otherEnemy = other.GetComponent<EnemyBase>();

            if (otherEnemy == null)
            {
                Debug.Log("몬스터 EnemyBase 가 없음");
                return;
            }

            if (ImFreeze == false && otherEnemy.ImFreeze == true)
            {

                otherEnemy.ImFreeze = false;
                otherEnemy.Move();

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 normal = (hitPoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(normal);
                Instantiate(CrashBunpeo, hitPoint, rot);


                PoolManager.Instance.DespawnNetworked(gameObject);

            }
        }

        else if (other.CompareTag("Aube"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Vector3 normal = (hitPoint - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(normal);

            Instantiate(CrashBunpeo, hitPoint, rot);


            PoolManager.Instance.DespawnNetworked(gameObject);

        }

    }

    public override void OnEnable()
    {
        base.OnEnable();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();


        terrain = Terrain.activeTerrain;

        float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f - 0.5f;
        transform.position = new Vector3(transform.position.x, terrainY, transform.position.z);

        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(true);
        }


        for (int i = 0; i < Fragment.Length; i++)
        {
            Fragment[i].SetActive(true);
        }

        StartCoroutine(WaitAndExplode());
    }

    private IEnumerator WaitAndExplode()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(AppearAni))
            yield return null;

        myCollider.enabled = true;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(explodeStateName));
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        Explode();
    }


    public override void OnDisable()
    {
        base.OnDisable();
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
            Fragment[i].SetActive(true);
        }
        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(true);
        }
        myCollider.enabled = false;
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

        Instantiate(explosionPartycle, transform.position, Quaternion.identity);

        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(false);
            myCollider.enabled = false;
        }


    }


    [PunRPC]
    public void IsActivateRPC()
    {
        //이거 파편 관련 된거임
        IsActivateFragment++;
        if (IsActivateFragment == 4)
        {

            IsActivateFragment = 0;


            PoolManager.Instance.DespawnNetworked(gameObject);

        }
    }
    public void IsActivate()
    {
        photonView.RPC("IsActivateRPC", RpcTarget.All);
    }

    public override void Move()
    {
        StartCoroutine(FreezeCor());
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction, isFreeze);
    }


    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {
        if (isFreeze == true)
        {
            ImFreeze = isFreeze;
            animator.speed = 0f;
            IsFreeze.SetActive(true);
        }
        else if (isFreeze == false)
        {
            Debug.Log(isFreeze + " 프리즈 해제");// 이거 넘어 오긴하는건가
            ImFreeze = isFreeze;
            StartCoroutine(FreezeCor());
        }
        else
        {
            Debug.Log("범퍼 프리즈 고장남");
        }
    }
    IEnumerator FreezeCor()
    {
        yield return new WaitForSeconds(FreezeTime);
        animator.speed = 1f;
        IsFreeze.SetActive(false);
    }

}

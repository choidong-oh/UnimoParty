using Photon.Pun;
using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.ParticleSystem;

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

    private string explodeStateName = "anim_MON003_ready02";

    [SerializeField] GameObject CrashBunpeo;

    Terrain terrain;

    int IsActivateFragment = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Manager.Instance.observer.HitPlayer(damage);
            Debug.Log(Manager.Instance.observer.UserPlayer.gamedata.life);

            Vector3 hitPoint = other.ClosestPoint(transform.position);//충돌지점에 최대한 가깝게

            Vector3 normal = (hitPoint - transform.position).normalized;// 방향계산
            Quaternion rot = Quaternion.LookRotation(normal);// 방향계산

            GameObject inst = Instantiate(CrashBunpeo, hitPoint, rot);


            gameObject.SetActive(false);
        }

    }

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    animator = GetComponent<Animator>();
    //    myCollider = GetComponent<Collider>();

    //    myCollider.enabled = true;

    //    terrain = Terrain.activeTerrain;

    //    float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f;
    //    transform.position = new Vector3(transform.position.x, terrainY, transform.position.z);

    //    for (int i = 0; i < Body.Length; i++)
    //    {
    //        Body[i].SetActive(true);
    //    }


    //    for (int i = 0; i < Fragment.Length; i++)
    //    {
    //        Fragment[i].SetActive(true);
    //    }

    //    StartCoroutine(WaitAndExplode());
    //}

    private IEnumerator WaitAndExplode()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(explodeStateName))
        {
            yield return null;
        }

        // State가 재생되는 순간, 연결된 클립길이 가져오기
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length == 0)
        {
            Debug.LogWarning($"{explodeStateName} 클립이 없음");
            yield break;
        }

        float clipLength = clips[0].clip.length;

        yield return new WaitForSeconds(clipLength);

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

        GameObject inst = Instantiate(explosionPartycle, transform.position, Quaternion.identity);

        for (int i = 0; i < Body.Length; i++)
        {
            Body[i].SetActive(false);
            myCollider.enabled = false;
        }


    }

    [PunRPC]
    public void IsActivateRPC()
    {
        IsActivateFragment++;
        if (IsActivateFragment == 4)
        {
            Debug.Log("완료");
            IsActivateFragment = 0;
            gameObject.SetActive(false);
        }
        Debug.Log(IsActivateFragment + "일단 작동함 ");
    }
    public void IsActivate()
    {
        photonView.RPC("IsActivateRPC", RpcTarget.All);
    }

    public override void Move(Vector3 direction)
    {
        photonView.RPC("MoveRPC", RpcTarget.All, direction);
    }

    public override void Freeze(Vector3 direction, bool isFreeze)
    {
        photonView.RPC("FreezeRPC", RpcTarget.All, direction);
    }

    public override void CsvEnemyInfo()
    {
        throw new System.NotImplementedException();
    }


    [PunRPC]
    public void MoveRPC(Vector3 direction)
    {
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();

        myCollider.enabled = true;

        terrain = Terrain.activeTerrain;

        float terrainY = terrain.SampleHeight(transform.position) + transform.localScale.y / 2f;
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


    [PunRPC]
    public void FreezeRPC(Vector3 direction, bool isFreeze)
    {

    }

}

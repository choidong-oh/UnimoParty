using Photon.Pun;
using System.Collections;
using UnityEngine;

public class FreezeBoom : MonoBehaviourPunCallbacks, IItemUse
{


    IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
        photonView.RPC("Explode", RpcTarget.All, true);

        yield return new WaitForSeconds(2);
        photonView.RPC("Explode", RpcTarget.All, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            StartCoroutine(wait());
        }

    }



    [PunRPC]
    void Explode(bool isFreeze)
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 30, Vector3.up, 100f, LayerMask.GetMask("Player", "Enemy", "Water"));
        foreach (var hitobj in hits)
        {
            Debug.Log("빙결 폭탄");

            GameObject target = hitobj.collider.gameObject;

            //플레이어 
            //왼손 : 움직임(조이스틱), 대시
            //오른손 : 그랩, 트리거, 아이템
            if (target.layer == LayerMask.NameToLayer("Player"))
            {
                JoystickController PlayerMove;
                HeadDash headDash;

                if ((PlayerMove = target.GetComponentInChildren<JoystickController>()) != null)
                {
                    PlayerMove.Freeze(isFreeze);
                    Debug.Log("플레이어 움직임 어름");
                }

                if ((headDash = target.GetComponentInChildren<HeadDash>()) != null)
                {
                    headDash.Freeze(isFreeze);

                    Debug.Log("플레이어 대시 어름");
                }

                if (target.TryGetComponent<HandHarvest>(out HandHarvest PlayerHarvest))
                {
                    PlayerHarvest.Freeze(isFreeze);

                    Debug.Log("플레이어 채집 어름");
                }

                //복구
                //IFreeze[] IFreezeInterface = GetComponentsInChildren<IFreeze>();
                //foreach (IFreeze temp in IFreezeInterface)
                //{
                //    temp.Freeze(true);

                //}

            }

            if (target.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (target.TryGetComponent<EnemyBase>(out EnemyBase Enemy))
                {
                    ICommand command = null;
                    command = new FreezeCommand(Enemy, transform.position, isFreeze);
                    command.Execute();



                    Debug.Log("enemy 어름");
                }
               
            }



        }
    }

    public void Use(Transform firepos, int power)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        Vector3 throwDirection = firepos.transform.forward + firepos.transform.up;
        transform.parent = null;
        rb.useGravity = true;
        rb.AddForce(throwDirection * power, ForceMode.VelocityChange);
        
        Collider cd = gameObject.GetComponent<BoxCollider>();
        cd.isTrigger = false;

        //Manager.Instance.observer.UseItem(ItemData selectitem);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////






}

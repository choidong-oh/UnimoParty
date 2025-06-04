using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FreezeBoom : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            photonView.RPC("Explode", RpcTarget.All);
            //Explode();
        }
    }

    [PunRPC]
    void Explode()
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
                    PlayerMove.Freeze(true);
                    Debug.Log("플레이어 움직임 어름");
                }

                if ((headDash = target.GetComponentInChildren<HeadDash>()) != null)
                {
                    headDash.Freeze(true);

                    Debug.Log("플레이어 대시 어름");
                }

                if (target.TryGetComponent<HandHarvest>(out HandHarvest PlayerHarvest))
                {
                    PlayerHarvest.Freeze(true);

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
                    command = new FreezeCommand(Enemy, transform.position, true);
                    command.Execute();



                    Debug.Log("enemy 어름");
                }
                if (target.TryGetComponent<TestEnemybase>(out TestEnemybase TestEnemy))
                {
                    TestEnemy.Freeze(transform.position,true);


                    Debug.Log("TestEnemy 어름");
                }

            }



        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////






}

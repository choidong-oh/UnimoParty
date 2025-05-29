using System.Collections;
using UnityEngine;

public class FreezeBoom : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(wait());
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
        Explode();
    }

    void Explode()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3, Vector3.up, 10f, LayerMask.GetMask("Player", "Enemy", "Water"));
        foreach (var hitobj in hits)
        {
            Debug.Log("빙결 폭탄");

            GameObject target = hitobj.collider.gameObject;

            //플레이어 
            //왼손 : 움직임(조이스틱)
            //오른손 : 그랩, 트리거, 아이템
            if (target.layer == LayerMask.NameToLayer("Player"))
            {
                //왼손
                if (target.TryGetComponent<JoystickController>(out JoystickController PlayerMove))
                {
                    PlayerMove.moveSpeed = 0;
                    Debug.Log("플레이어 움직임 어름");
                }


                if (target.TryGetComponent<HandHarvest>(out HandHarvest PlayerHarvest))
                {
                    PlayerHarvest.Freeze(false);

                    Debug.Log("플레이어 채집 어름");
                }


            }

            if (target.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (target.TryGetComponent<EnemyBase>(out EnemyBase Enemy))
                {
                    ICommand command = null;
                    command = new FreezeCommand(Enemy, transform.position);
                    command.Execute();



                    Debug.Log("enemy 어름");
                }
                if (target.TryGetComponent<TestEnemybase>(out TestEnemybase TestEnemy))
                {
                    TestEnemy.Freeze(transform.position);


                    Debug.Log("TestEnemy 어름");
                }

            }



        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////
    





}

using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IAttackable
{
    int Hp;
    public void TakeDamage(int Dmg)
    {
        Hp -= Dmg;
        if (Hp <= 0)
        {
            //DIE
        }
    }
}


public interface IAttackable
{
    void TakeDamage(int Dmg);
}






//예시
public class Enemy1 : EnemyBase
{
    int Power;

    //플레이어와 상호작용했을때 가정
    private void OnCollisionEnter(Collision collision)
    {
        //player.TakeDamage(Power);
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//나중에 player틀 짜는사람 복붙하면댐

public class PlayerGetDmg : MonoBehaviour,IDamageable
{
    [SerializeField]int hp = 10;
    public void TakeDamage(int dmg)
    {
        hp-=dmg;    
        if(hp <= 0)
        {
            //플레이어 죽음
            //this.gameObject.transform.parent.gameObject.SetActive(false);   
            this.gameObject.transform.gameObject.SetActive(false);   
        }

    }


    [ContextMenu("테스트 데미지 받기")]
    public void testTakeDmgBtn()
    {
        TakeDamage(5);
    }
   
}

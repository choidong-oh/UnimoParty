using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCommand : ICommand
{
    EnemyBase enemyBase;
    Vector3 direction;
    bool isFreeze;
    public float DelayTime => throw new System.NotImplementedException();


    public FreezeCommand(EnemyBase enemyBase, Vector3 direction, bool isFreeze)
    {
        this.enemyBase = enemyBase;
        this.direction = direction;
        this.isFreeze = isFreeze;   
        
    }


    public void Execute()
    {
        enemyBase.Freeze(direction,isFreeze);
    }
}

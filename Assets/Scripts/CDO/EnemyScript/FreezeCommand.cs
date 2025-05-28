using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCommand : ICommand
{
    EnemyBase enemyBase;
    Vector3 direction;
    public float DelayTime => throw new System.NotImplementedException();


    public FreezeCommand(EnemyBase enemyBase, Vector3 direction)
    {
        this.enemyBase = enemyBase;
        this.direction = direction;
    }


    public void Execute()
    {
        //enemyBase.Freeze(direction);
    }
}

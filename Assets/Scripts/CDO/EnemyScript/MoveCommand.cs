using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    EnemyBase enemyBase;
    Vector3 direction;

    public float DelayTime { get; private set; }

    public MoveCommand(EnemyBase enemyBase, Vector3 direction, float delayTime)
    {
        this.enemyBase = enemyBase;
        this.direction = direction;
        DelayTime = delayTime;
    }

    public void Execute()
    {
        enemyBase.Move(direction);
    }

}

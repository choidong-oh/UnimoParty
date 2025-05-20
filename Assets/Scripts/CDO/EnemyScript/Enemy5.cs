using System.Collections;
using UnityEngine;

//receiver
public class Enemy5 : EnemyBase, IDamageable
{
    public float moveSpeed = 5f;
   
    public override void Move(Vector3 direction)
    {
        Debug.Log("Enemy5 move 실행");
        transform.Translate(direction * moveSpeed);
    }
}
public class Enemy4 : EnemyBase, IDamageable
{
    public float moveSpeed = 15f;

    public override void Move(Vector3 direction)
    {
        Debug.Log("Enemy4 move 실행");
        transform.Translate(-direction * moveSpeed);
    }
}

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

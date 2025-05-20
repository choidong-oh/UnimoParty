using System.Collections;
using UnityEngine;

public class Enemy5 : EnemyBase, IDamageable
{
    Vector3 direction;
    Vector3 startPos;
    public float moveSpeed = 5f;

    CommandReplay commandReplay;

    //**다른스크립트에서 볼수있음, 근데 내부에서만 변경 가능? 프로텍트?
    //그냥 신기해서 가져옴
    public float DelayTime { get; private set; }

    private void Start()
    {
        startPos = transform.position;

        commandReplay = new CommandReplay();
        StartCoroutine(cor());

    }

    IEnumerator cor()
    {
        ExecuteMoveCommand(Vector3.forward, 5f, 0f);
        yield return new WaitForSeconds(2f);
        ExecuteMoveCommand(Vector3.forward, 5f, 2f);
        yield return new WaitForSeconds(3f);

        transform.position = startPos;
    }

    private void ExecuteMoveCommand(Vector3 direction, float speed, float delay)
    {
        ICommand moveCommand = new MoveCommand(this, direction, speed);
        moveCommand.Execute();

        //commandReplay.ExecuteCommand(moveCommand);
    }

    public override void Move(Vector3 direction)
    {
        Debug.Log("move 실행");
        transform.Translate(direction * moveSpeed);
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

public class Enemy4 : EnemyBase, IDamageable
{
    private void Start()
    {
        ExecuteMoveCommand(Vector3.forward, 5f, 0f);
    }

    public override void Move(Vector3 direction)
    {
        //오른쪽으로 감
    }

    //중복이 일어나나?
    private void ExecuteMoveCommand(Vector3 direction, float speed, float delay)
    {
        ICommand moveCommand = new MoveCommand(this, direction, speed);
        moveCommand.Execute();

        //commandReplay.ExecuteCommand(moveCommand);
    }

}
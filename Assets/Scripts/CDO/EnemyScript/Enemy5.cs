using System.Collections;
using UnityEngine;


//질문1. DelayTime이거는 내가 생각한게 맞는지?
//질문2. 원래 행동별로 나눠야대는데 틀만 잡을려고 하다보니 이렇게됨
//       이게 커맨드패턴 맞나?  캡슐화한건 맞는데 이상함
//질문3. 의도대로 할려면 기능 나눠야하는거같은데 그러면 헷갈리지안나? 
//       기능 넣으면 기본 직선적1, 곡선적2이 있음 //총3을 상속받음
//       행동직선4, 행동곡선5 이렇게 5개 만들어야하나?
//       추가로 공격있으면 공격직선6, 공격곡선7 이렇게돼나?
//질문4. 커맨드 vs 상태

/// <summary>
/// //////////////////////////////////////////////////////////////
/// </summary>


//[Invoker] CommandReplay
//    |
//    V
//[ICommand] <interface>
//    | -float DelayTime { get; }
//    | -void Execute()

//[Command] MoveCommand: ICommand
//    | -EnemyBase receiver
//    | -Vector3 direction

//[Receiver] Enemy5: EnemyBase
//    | - public void Move(Vector3 direction)
public class Enemy5 : EnemyBase, IDamageable, ICommand
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
        move1(0);
        yield return new WaitForSeconds(2f);
        move1(2);
        yield return new WaitForSeconds(3f);

        transform.position = startPos;
        yield return new WaitForSeconds(1f);

        //commandInvoker.ReplayCommands();
        yield return StartCoroutine(commandReplay.ReplayCommandsCoroutine());
    }

    private void move1(float delay)
    {
        Vector3 moveDir = Vector3.forward;
        SetMoveCommand(moveDir, delay);

        //Execute(); //리플레이 안쓰면
        commandReplay.ExecuteCommand(this); //리플레이용
    }


    public override void Move(Vector3 direction)
    {
        transform.Translate(direction * moveSpeed);
    }

    public void SetMoveCommand(Vector3 direction, float delay)
    {
        this.direction = direction;
        DelayTime = delay;
    }

    public void Execute()
    {
        Move(direction);
    }
}
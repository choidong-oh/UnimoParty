using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//invoker
public class GameController : MonoBehaviour
{
    ICommand command;

    // 커맨드 설정
    public void SetCommand(ICommand command)
    {
        this.command = command;
    }

    // 커맨드 실행
    public void ExecuteCommand()
    {
        command.Execute();
    }
}

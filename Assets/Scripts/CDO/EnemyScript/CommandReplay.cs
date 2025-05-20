using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//invoker
//리플레이용
//해보고 싶어서 해봄
public class CommandReplay
{
    //기록용
    private Queue<ICommand> commandHistory = new Queue<ICommand>();


    public void ExecuteCommand(ICommand newCommand)
    {
        commandHistory.Enqueue(newCommand);
        newCommand.Execute();
    }

    public void ReplayCommands()
    {
        Debug.Log(commandHistory.Count);
        foreach (var cmd in commandHistory)
        {
            Debug.Log("2");
            cmd.Execute();
        }
    }

    public IEnumerator ReplayCommandsCoroutine()
    {
        Debug.Log("리플레이 들어옴");
        foreach (var cmd in commandHistory)
        {
            yield return new WaitForSeconds(cmd.DelayTime); 
            cmd.Execute();
        }
    }

    public void ClearHistory()
    {
        commandHistory.Clear();
    }

    
}

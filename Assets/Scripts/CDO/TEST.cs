using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TEST : MonoBehaviour
{
    public CommandReplay commandReplay;


    private void Start()
    {
        commandReplay = new CommandReplay();
    }
    public void replay()
    {
        StartCoroutine(cor());
    }

    IEnumerator cor()
    {
        yield return StartCoroutine(commandReplay.ReplayCommandsCoroutine());
    }


}



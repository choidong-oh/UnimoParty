using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestA : MonoBehaviour
{
    [SerializeField]
    private InputActionReference InputActionReference;

    private void OnEnable()
    {
        InputActionReference.action.performed += dd;

    }

    private void OnDisable()
    {
        InputActionReference.action.performed -= dd;

    }

    private void dd(InputAction.CallbackContext context)
    {
        Debug.Log("키눌름 뭔지는모름");
    }

    
}

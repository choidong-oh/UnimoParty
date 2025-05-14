using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TEST : MonoBehaviour
{
    [SerializeField] private InputActionReference activateAction;

    private void OnEnable()
    {
        activateAction.action.performed += OnTriggerPressed;
        activateAction.action.canceled += OnTriggerReleased;
        activateAction.action.Enable();
    }

    private void OnDisable()
    {
        activateAction.action.performed -= OnTriggerPressed;
        activateAction.action.canceled -= OnTriggerReleased;
        activateAction.action.Disable();
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger ´­¸²");
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger ¶À");
    }






}



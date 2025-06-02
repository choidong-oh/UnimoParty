using UnityEngine;
using UnityEngine.InputSystem;


//secondaryButton[오른손 XR 컨트롤러] = B 버튼
//primaryButton[오른손 XR 컨트롤러] = A 버튼
public class ItemInputB : MonoBehaviour, IFreeze
{
    [SerializeField] InputActionReference BInputActionReference;

    [SerializeField] InputActionReference triggerInputActionReference;

    private void OnEnable()
    {
        BInputActionReference.action.Enable();
        BInputActionReference.action.performed += ControllerB;

        triggerInputActionReference.action.performed += OnTriggerPressed;
        triggerInputActionReference.action.canceled += OnTriggerReleased;
    }

    private void OnDisable()
    {
        BInputActionReference.action.performed -= ControllerB;
        BInputActionReference.action.Disable();

        triggerInputActionReference.action.performed -= OnTriggerPressed;
        triggerInputActionReference.action.canceled -= OnTriggerReleased;
    }

    //트리거 함
    private void OnTriggerPressed(InputAction.CallbackContext context)
    { 
        //아이템 사용


    }

    //트리거 뗌
    void OnTriggerReleased(InputAction.CallbackContext context)
    {



    }

    //아이템 교체 b
    private void ControllerB(InputAction.CallbackContext context)
    {
        Debug.Log("컨트롤 b버튼, 아이템 교체");
    }

    public void Freeze(bool IsFreeze)
    {
        throw new System.NotImplementedException();
    }
}

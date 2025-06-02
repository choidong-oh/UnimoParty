using UnityEngine;
using UnityEngine.InputSystem;


//secondaryButton[오른손 XR 컨트롤러] = B 버튼
//primaryButton[오른손 XR 컨트롤러] = A 버튼
public class TestA : MonoBehaviour
{
    [SerializeField]
    private InputActionReference AInputActionReference;
    private InputActionReference BInputActionReference;

    private void OnEnable()
    {
        AInputActionReference.action.Enable();
        AInputActionReference.action.performed += ControllerA;

        BInputActionReference.action.Enable();
        BInputActionReference.action.performed += ControllerB;
    }

    private void OnDisable()
    {
        AInputActionReference.action.performed -= ControllerA;
        AInputActionReference.action.Disable();

        BInputActionReference.action.performed -= ControllerB;
        BInputActionReference.action.Disable();
    }

    //아이템, 채집총 교체 a
    private void ControllerA(InputAction.CallbackContext context)
    {
        Debug.Log("컨트롤 a버튼");
    }

    //아이템 교체 b
    private void ControllerB(InputAction.CallbackContext context)
    {
        Debug.Log("컨트롤 b버튼");
    }









}

using UnityEngine;
using UnityEngine.InputSystem;


//secondaryButton[오른손 XR 컨트롤러] = B 버튼
//primaryButton[오른손 XR 컨트롤러] = A 버튼
public class TestA : MonoBehaviour
{
    [SerializeField]
    private InputActionReference BInputActionReference;

    private void OnEnable()
    {
        BInputActionReference.action.Enable();
        BInputActionReference.action.performed += ControllerB;
    }

    private void OnDisable()
    {
        BInputActionReference.action.performed -= ControllerB;
        BInputActionReference.action.Disable();
    }



    //아이템 교체 b
    private void ControllerB(InputAction.CallbackContext context)
    {
        Debug.Log("컨트롤 b버튼");
    }









}

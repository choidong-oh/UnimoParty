using UnityEngine;
using UnityEngine.InputSystem;  // Áß¿ä

public class SimpleMover : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        Vector2 input = Keyboard.current != null
            ? new Vector2(
                (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0)
              )
            : Vector2.zero;

        Vector3 direction = new Vector3(input.x, 0f, input.y);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}

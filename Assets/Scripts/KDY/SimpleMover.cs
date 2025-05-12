using UnityEngine;
using UnityEngine.InputSystem; // New Input System »ç¿ë

public class SimpleMover : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 input = Keyboard.current != null
            ? new Vector2(
                (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0)
              )
            : Vector2.zero;

        Vector3 move = new Vector3(input.x, 0, input.y);
        rb.MovePosition(transform.position + move * speed * Time.deltaTime);
    }
}

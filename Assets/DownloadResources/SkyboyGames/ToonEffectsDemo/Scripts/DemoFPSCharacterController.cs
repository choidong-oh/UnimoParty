using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoFPSCharacterController : MonoBehaviour
{
    public float MovementSpeed = 10.0f;
    public float MouseSensitivity = 5.0f;

    Camera MyCamera;
    Vector2 MouseLookAmount;

    // Start is called before the first frame update
    void Start()
    {
        MyCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime);

        if(MyCamera != null)
        {
            MouseLookAmount.x += Input.GetAxisRaw("Mouse X") * MouseSensitivity;
            MouseLookAmount.y += Input.GetAxisRaw("Mouse Y") * MouseSensitivity;

            MyCamera.transform.localRotation = Quaternion.AngleAxis(-Mathf.Clamp(MouseLookAmount.y, -45.0f, 60.0f), Vector3.right);
            transform.localRotation = Quaternion.AngleAxis(MouseLookAmount.x, transform.up);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

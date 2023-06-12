using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] float sensitivityX = 1000f;
    [SerializeField] float sensitivityY = 1000f;
    [SerializeField] float mouseLookSmooth = 0.05f;

    [Header("References")]
    [SerializeField] Transform playerBody;

    float mouseX;
    float mouseY;
    float xRotation = 0f;
    float yRotation = 0f;
    float mouseLookDampX = 0.0f;
    float mouseLookDampY = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        GetInput();
        
        // camera rotation
        // transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.localEulerAngles = new Vector3(xRotation, yRotation, 0f);

        // horizontal player visuals rotation
        // playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
        playerBody.localEulerAngles = new Vector3(0f, yRotation, 0f);

        // keep camera up with player visuals
        transform.position = playerBody.position + new Vector3(0, 0.9f, 0);
    }

    void GetInput()
    {
        // position of mouse cursor
        mouseX += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        // how much camera should rotate on the x-axis (up & down)
        // negative x-rotation looks up so we subtract the mouseY position (positive is moving mouse up)
        xRotation = Mathf.SmoothDamp(xRotation, mouseY, ref mouseLookDampX, mouseLookSmooth);
        yRotation = Mathf.SmoothDamp(yRotation, mouseX, ref mouseLookDampY, mouseLookSmooth);
    }
}

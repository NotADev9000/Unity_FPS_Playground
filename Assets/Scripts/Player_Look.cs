using UnityEngine;

public class Player_Look : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] Transform playerCamera;
    
    [Header("Look Settings")]
    [SerializeField] float sensitivityX = 200f;
    [SerializeField] float sensitivityY = 200f;
    [SerializeField] float mouseLookSmooth = 0.05f;

    // mouse parameters
    float mouseX;
    float mouseY;
    float currentRotationX;
    float currentRotationY;
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

        // horizontal player rotation
        transform.localEulerAngles = new Vector3(0f, currentRotationY, 0f);

        // vertical camera rotation
        playerCamera.localEulerAngles = new Vector3(currentRotationX, 0f, 0f);
    }

    void GetInput()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // position of mouse cursor
            mouseX += Input.GetAxisRaw("Mouse X") * sensitivityX * 0.01f;
            mouseY -= Input.GetAxisRaw("Mouse Y") * sensitivityY * 0.01f;
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);

            // smooth mouse rotation (prevents jitter)
            currentRotationX = Mathf.SmoothDamp(currentRotationX, mouseY, ref mouseLookDampX, mouseLookSmooth);
            currentRotationY = Mathf.SmoothDamp(currentRotationY, mouseX, ref mouseLookDampY, mouseLookSmooth);
        }
    }
}

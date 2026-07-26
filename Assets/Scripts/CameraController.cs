using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float cameraClampY = 90f;
    [SerializeField] private float smoothTime = 15f;

    [Header("References")]
    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private float smoothX;
    private float smoothY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // // Get raw mouse input (no Time.deltaTime)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Apply rotation deltas
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -cameraClampY, cameraClampY);

        // Smooth the camera rotation
        smoothX = Mathf.Lerp(smoothX, xRotation, smoothTime * Time.deltaTime);
        smoothY = Mathf.Lerp(smoothY, yRotation, smoothTime * Time.deltaTime);

        // Apply smoothed rotation to camera
        transform.rotation = Quaternion.Euler(smoothX, smoothY, 0f);

        // Instantly rotate player body (so movement direction updates immediately)
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
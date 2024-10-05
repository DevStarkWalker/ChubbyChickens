using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;
    public float distanceFromPlayer = 5f;
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 40f;
    public float rotationSpeed = 5f;
    public LayerMask groundLayer;

    public float xRotation = 0f;
    public float currentYaw = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        HandleCameraRotation();
        UpdateCameraPosition();
        PreventGroundClipping();
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Adjust yaw rotation smoothly
        currentYaw += mouseX;

        // Wrap currentYaw to stay within -180 to 180 range
        if (currentYaw > 180f)
        {
            currentYaw -= 360f;
        }
        else if (currentYaw < -180f)
        {
            currentYaw += 360f;
        }

        transform.localRotation = Quaternion.Euler(xRotation, currentYaw, 0f);
    }

    void UpdateCameraPosition()
    {
        Vector3 direction = transform.rotation * Vector3.back;
        transform.position = player.position - direction * distanceFromPlayer;
    }

    void PreventGroundClipping()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, -transform.forward, out hit, distanceFromPlayer, groundLayer))
        {
            transform.position = hit.point + transform.forward * 0.5f;
        }
    }

    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    public float GetCameraYaw()
    {
        return currentYaw;
    }
}
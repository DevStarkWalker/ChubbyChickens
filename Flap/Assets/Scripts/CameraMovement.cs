using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // Reference to the player transform
    public float distanceFromPlayer = 5f;
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 40f;
    public float minYaw = -90f; // Min yaw rotation limit
    public float maxYaw = 90f;  // Max yaw rotation limit
    public LayerMask groundLayer; // Layer for ground detection

    private float xRotation = 0f;
    private float currentYaw = 0f;
    private Vector3 currentOffset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentOffset = new Vector3(0f, 0f, -distanceFromPlayer);
    }

    void LateUpdate()
    {
        HandleCameraRotation();
        UpdateCameraPosition();
        PreventGroundClipping();
    }

    void HandleCameraRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate camera up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Swivel camera around player with limits
        currentYaw += mouseX;
        currentYaw = Mathf.Clamp(currentYaw, minYaw, maxYaw);

        // Apply rotation
        transform.localRotation = Quaternion.Euler(xRotation, currentYaw, 0f);
    }

    void UpdateCameraPosition()
    {
        // Calculate the new position of the camera based on player position and rotation
        transform.position = player.position + transform.rotation * currentOffset;
    }

    void PreventGroundClipping()
    {
        // Raycast from the camera to the player to check if the camera is about to clip through the ground
        RaycastHit hit;
        if (Physics.Raycast(player.position, -transform.forward, out hit, distanceFromPlayer, groundLayer))
        {
            // Adjust camera position to avoid clipping through the ground
            transform.position = hit.point + transform.forward * 0.5f; // Offset slightly forward
        }
    }

    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0; // Flatten the forward vector
        return forward.normalized;
    }
}


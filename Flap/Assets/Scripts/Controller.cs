using UnityEngine;

public class Controller : MonoBehaviour
{

    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float glideGravity = -2f;  // Reduced gravity for gliding
    public bool canMove = false;
    [Header("References")]
    public CameraMovement cameraMovement;
    public Animator animator;
    private Rigidbody rb;
    public bool isMoving;
    public bool isGrounded;
    public bool isSprinting;
    public bool isGliding;


    private bool jumpPeakReached;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent the Rigidbody from rotating on its own

    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, .5f);

        if (canMove)
        {
            HandleMovement();
            HandleJumpAndGravity();
        }

        HandleAnimation();
       
    }

    void HandleMovement()
    {

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = cameraMovement.GetCameraForward() * moveZ + transform.right * moveX;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        // Trigger the "isWalking" animation if there's movement

        isMoving = move.magnitude > 0.1f;

        if (isMoving)
        { 
            Quaternion targetRotation = Quaternion.Euler(0f, cameraMovement.transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.05f);
            cameraMovement.mouseSensitivity = 5;
            // Apply movement
            rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
            // Reset mouse input to the center of the screen to prevent continuous spinning
            Cursor.lockState = CursorLockMode.Locked;
        }

        
    }

    void HandleAnimation()
    {
        if (!isGrounded && !isGliding)
        {
            animator.Play("Spin");
        }
        if (!isGrounded && isGliding)
        {
            animator.Play("Fly");
        }
        if (isGrounded && isMoving && !isSprinting)
        {
            animator.Play("Walk");
        }
        if (isGrounded && isMoving && isSprinting)
        {
            animator.Play("Run");
        }
        if (isGrounded && !isMoving)
        {
            animator.Play("Idle_A");
        }
    }

    void HandleJumpAndGravity()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            jumpPeakReached = false;
            isGliding = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Apply gravity manually
        if (!isGrounded)
        {
            if (!jumpPeakReached && rb.velocity.y <= 0)
            {
                // Player is at the peak of the jump
                jumpPeakReached = true;

                // Check if the player holds space to start gliding
                isGliding = Input.GetButton("Jump");
            }

            float currentGravity = isGliding ? glideGravity : gravity;
            rb.AddForce(Vector3.up * currentGravity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            isMoving = false;
            GameManager.Instance.FinishedLevel();
            other.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Debug.Log("You Suck");
            animator.Play("Death");
            GameManager.Instance.GameOver();

        }
    }
}




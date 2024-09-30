using UnityEngine;

public class Player2DController : MonoBehaviour
{
    // Movement variables
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 10f;
    public float glideGravity = 0.5f;  // Gravity scale while gliding
    public LayerMask groundLayer = 3;  // Layer mask to identify ground

    // Components
    [SerializeField] private Rigidbody2D rb;
    public Animator animator;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isGliding = false;

    // Ground check
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;  // Distance for the raycast ground check

    // Original gravity to revert after gliding
    private float originalGravity;

    // Delay to prevent immediate ground check after jumping
    private float groundCheckDelay = 0.1f;
    private float groundCheckTimer = 0f;

    // Variable to track which direction the player is facing
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale; // Store the original gravity scale
    }

    private void Update()
    {
        // If jumping, start a timer to delay ground checks
        if (isJumping)
        {
            groundCheckTimer -= Time.deltaTime;
        }

        // Only check for ground if not jumping or if the ground check delay has passed
        if (!isJumping || groundCheckTimer <= 0)
        {
            isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        }

        // Horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        float currentSpeed = moveSpeed;

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;  // Increase speed while sprinting
        }

        // Apply horizontal movement
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        // Handle turning to face the direction of movement
        Flip(moveInput);

        // Handle animations
        HandleAnimations(moveInput, currentSpeed);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            animator.SetTrigger("Jump");
            animator.SetBool("Grounded", false);
            isGrounded = false;
            groundCheckTimer = groundCheckDelay;  // Start delay timer
        }

        // Glide (while holding Space in the air)
        if (Input.GetKey(KeyCode.Space) && !isGrounded && !isGliding && rb.velocity.y <= 0)
        {
            StartGlide();
        }
        else if (Input.GetKeyUp(KeyCode.Space) || isGrounded) // Stop gliding if Space is released or grounded
        {
            StopGlide();
        }

        // Reset jumping state if grounded
        if (isGrounded && !isJumping)
        {
            isJumping = false;
            animator.SetBool("Grounded", true);
            animator.SetBool("IsGliding", false);  // End glide animation if grounded
        }
    }

    private void HandleAnimations(float moveInput, float currentSpeed)
    {
        if (Mathf.Abs(moveInput) == 0)
        {
            animator.SetBool("NotMoving", true);
        }

        // Check for walking animation
        if (Mathf.Abs(moveInput) > 0 && !Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            animator.SetBool("NotMoving", false);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);  // Not running
        }
        // Check for running animation
        else if (Mathf.Abs(moveInput) > 0 && Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            animator.SetBool("IsWalking", false);  // Stop walking
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);  // Stop walking
            animator.SetBool("IsRunning", false);  // Stop running
        }
    }

    private void StartGlide()
    {
        animator.SetBool("IsGliding", true);  // Trigger glide animation
        isGliding = true;
        rb.gravityScale = glideGravity;  // Reduce gravity for gliding effect
    }

    private void StopGlide()
    {
        isGliding = false;
        rb.gravityScale = originalGravity;  // Restore original gravity scale
        animator.SetBool("IsGliding", false);  // End glide animation
    }

    // Flip the character based on the direction of movement
    private void Flip(float moveInput)
    {
        // If moving to the right and facing left, flip the player
        if (moveInput > 0 && !facingRight)
        {
            facingRight = true;
            Vector3 theScale = transform.localScale;
            theScale.x = Mathf.Abs(theScale.x);  // Set x scale to positive (facing right)
            transform.localScale = theScale;
        }
        // If moving to the left and facing right, flip the player
        else if (moveInput < 0 && facingRight)
        {
            facingRight = false;
            Vector3 theScale = transform.localScale;
            theScale.x = -Mathf.Abs(theScale.x);  // Set x scale to negative (facing left)
            transform.localScale = theScale;
        }
    }

    // Optional: Visualize the raycast in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            animator.Play("Idle_A");
            GameManager.Instance.FinishedLevel();
            collision.enabled = false;
        }
        if (collision.gameObject.tag == "Floor")
        {
            rb.gravityScale = 0;
            rb.velocity = Vector3.zero;
            GameManager.Instance.GameOver();
        }
    }
}


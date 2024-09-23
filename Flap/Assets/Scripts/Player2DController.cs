using UnityEngine;

public class Player2DController : MonoBehaviour
{
    // Movement variables
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 0f;
    public float glideGravity = 0.5f;  // Gravity scale while gliding
    public LayerMask groundLayer = 3;      // Layer mask to identify ground

    // Components
    [SerializeField] private Rigidbody2D rb;
    public Animator animator;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isGliding = false;

    // Ground check
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    // Original gravity to revert after gliding
    private float originalGravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale; // Store the original gravity scale
    }

    private void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

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

        // Handle animations
        HandleAnimations(moveInput, currentSpeed);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            animator.SetTrigger("Jump");
            animator.SetBool("Grounded", false);
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
        // If grounded, reset the jump and glide states
        if (isGrounded)
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
        animator.SetBool("IsGliding", false);  // Trigger glide animation
    }

    // Optional: Visualize ground check in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

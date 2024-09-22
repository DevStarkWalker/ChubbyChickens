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

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }

        // Glide (while holding Space in the air)
        if (Input.GetKey(KeyCode.Space) && !isGrounded && !isGliding && rb.velocity.y < 0)
        {
            StartGlide();
        }
        else if (Input.GetKeyUp(KeyCode.Space) || isGrounded) // Stop gliding if Space is released or grounded
        {
            StopGlide();
        }
    }

    private void StartGlide()
    {
        isGliding = true;
        rb.gravityScale = glideGravity;  // Reduce gravity for gliding effect
    }

    private void StopGlide()
    {
        isGliding = false;
        rb.gravityScale = originalGravity;  // Restore original gravity scale
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform;
    public Animator animator;
    public Transform groundCheck;

    [Header("Adjustments")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float glideGravityMultiplier = 0.3f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f; // Time to smooth player turning
    public float groundDistance; 
    public LayerMask groundMask;
    

    [Header("Booleans")]
    public bool isGrounded;
    public bool isGliding = false;


    private float currentSpeed;
    private Vector3 velocity;
    private float turnSmoothVelocity; // Helper variable for smoothing rotation
    

    void Update()
    {
        // Ground Check
        isGrounded = Physics.Raycast(groundCheck.position, -Vector3.up, groundDistance, groundMask);
        Debug.DrawRay(groundCheck.position, -Vector3.up, Color.red);
        animator.SetBool("Grounded", isGrounded);


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset Y velocity when grounded
        }

        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Adjust movement speed for running or walking
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
            animator.SetBool("isRunning", direction.magnitude > 0);
        }
        else
        {
            currentSpeed = walkSpeed;
            animator.SetBool("isRunning", false);
        }

        animator.SetBool("isWalking", direction.magnitude > 0);
        animator.SetBool("NotMoving", direction.magnitude == 0);

        // Move the player if there is input
        if (direction.magnitude >= 0.1f)
        {
            // Get the target angle based on camera direction and input direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Smooth the player rotation
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotate the player to face the target direction
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            // Move the player in the direction the camera is facing
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // Glide
        if (Input.GetButton("Jump") && !isGrounded)
        {
            isGliding = true;
            velocity.y += gravity * glideGravityMultiplier * Time.deltaTime; // Reduce fall speed
            animator.SetBool("isGliding", true);
        }
        else
        {
            isGliding = false;
            animator.SetBool("isGliding", false);
        }

        if (Input.GetButtonUp("Jump"))
        {
            isGliding = false;
            animator.SetBool("isGliding", false);
        }

        // Apply gravity when not gliding
        if (!isGliding)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Apply vertical movement (gravity and jump velocity)
        controller.Move(velocity * Time.deltaTime);
    }
}

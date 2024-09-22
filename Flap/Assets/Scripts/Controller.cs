using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 _move;
    public Vector2 _look;

    public Vector3 nextPosition;
    public Quaternion nextRotation;

    public float rotationPower = 3f;
    public float rotationLerp = 0.5f;

    public float speed = 1f;

    private Vector2 smoothDeltaPosition = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    public float magnitude = 0.25f;  // Threshold to consider player as "moving"
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float glideGravity = -2f;

    [Header("References")]
    public Animator animator;
    public Camera camera;
    public GameObject followTransform;
    private Rigidbody rb;

    [Header("Booleans")]
    public bool canMove = false;
    public bool isMoving;
    public bool isGrounded;
    public bool isSprinting;
    public bool isGliding;

    private bool jumpPeakReached;

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
    }

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        #region Movement Related Physics
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.5f);

        Vector3 worldDeltaPosition = nextPosition - transform.position;

        // Map to local space
        float dX = Vector3.Dot(transform.right, worldDeltaPosition);
        float dY = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dX, dY);

        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        if (Time.deltaTime > 1e-5f)
        {
            velocity = smoothDeltaPosition / Time.deltaTime;
        }

        // Check if velocity is significant enough to consider the player as moving
        isMoving = velocity.magnitude > magnitude;

        #endregion

        #region Camera Rotation Related Logic
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.y * rotationPower, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        // Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTransform.transform.localEulerAngles = angles;

        #endregion

        nextRotation = Quaternion.Lerp(followTransform.transform.rotation, nextRotation, Time.deltaTime * rotationLerp);

        if (_move.x == 0 && _move.y == 0)
        {
            nextPosition = transform.position;

            return;
        }
        float moveSpeed = speed / 100f;
        Vector3 position = (transform.forward * _move.y * moveSpeed) + (transform.right * _move.x * moveSpeed);
        nextPosition = transform.position + position;


        //Set the player rotation based on the look transform
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        HandleAnimation();
        HandleJumpAndGravity();
    }

    private void OnAnimatorMove()
    {
        //Update the position based on the next position;
        transform.position = nextPosition;
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

        if (!isGrounded)
        {
            if (!jumpPeakReached && rb.velocity.y <= 0)
            {
                jumpPeakReached = true;
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
            animator.Play("Idle_A");
            GameManager.Instance.FinishedLevel();
            other.enabled = false;
        }
        if (other.gameObject.tag == "Floor")
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            GameManager.Instance.GameOver();
        }
    }
}


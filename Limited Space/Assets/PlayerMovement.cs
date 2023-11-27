using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed at which the player moves.")]
    public float moveSpeed = 5.0f;

    [Tooltip("Strength of the jump.")]
    public float jumpForce = 5.0f;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private bool isGrounded;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerControls.FPSPlayerActions.Enable();
        playerControls.FPSPlayerActions.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        playerControls.FPSPlayerActions.Disable();
        playerControls.FPSPlayerActions.Jump.performed -= OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        Move();
        CheckGrounded();
    }

    private void Move()
    {
        Vector2 inputVector = playerControls.FPSPlayerActions.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        moveDirection = transform.TransformDirection(moveDirection);
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void CheckGrounded()
    {
        // Adjust ground check distance as necessary.
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
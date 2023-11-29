using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// This script provides advanced first-person character controls.
// It includes custom gravity handling, air control, head bobbing, and camera rotation during strafing.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class AdvancedPlayerMovement : MonoBehaviour
{
    // Input and movement
    private PlayerControls playerControls;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private Vector3 originalScale; // Original scale of the player
    private float originalColliderHeight; // Original collider height
    private float originalColliderRadius; // Original collider radius
    private float originalGravityScale; // Original gravity scale
    private float originalFallMultiplier; // Original fall multiplier
    private float originalJumpForce; // Original jump force
    private float originalMoveSpeed; // Original movement speed

    [SerializeField] private GameObject playerWeapon; // Reference to the player's weapon
    private Vector3 originalWeaponScale; // To store the original scale of the weapon

    // Gravity and Jumping
    [Header("Gravity and Jump Settings")]
    [SerializeField, Tooltip("Custom gravity scale")]
    private float gravityScale = 2.0f;
    [SerializeField, Tooltip("Multiplier for falling")]
    private float fallMultiplier = 2.5f;
    [SerializeField, Tooltip("Jump force")]
    private float jumpForce = 7.0f;
    [SerializeField, Tooltip("Cooldown between jumps")]
    private float jumpCooldown = 0.1f;
    [SerializeField, Tooltip("Air control multiplier")]
    private float airControl = 0.5f;
    [SerializeField, Tooltip("Maximum number of jumps allowed")]
    private int maxJumpCount = 1;
    [SerializeField, Tooltip("Layer mask for what is considered ground")]
    private LayerMask groundLayer;
    [SerializeField, Tooltip("Distance to check for ground")]
    private float groundCheckDistance = 0.1f;

    // Ground checking
    private float groundedCheckDelay = 0.05f;
    private float lastTimeGrounded;
    private float groundCheckRadius = 0.3f; // Radius for SphereCast

    // Jump state
    private int currentJumpCount = 0;
    private bool canJump = true;
    private bool wasInAir = false;
    public bool isGrounded;
    private float lastJumpTime;

    // Movement settings
    [Header("Movement Settings")]
    [SerializeField, Tooltip("Base movement speed")]
    private float moveSpeed = 5.0f;
    [SerializeField, Tooltip("Sprint speed multiplier")]
    private float sprintMultiplier = 1.5f;
    [SerializeField, Tooltip("Rotation speed for camera adjustments")]
    private float rotationSpeed = 5.0f;

    // Crouching settings
    [Header("Crouch Settings")]
    [SerializeField, Tooltip("Height while crouching")]
    private float crouchHeight = 1.0f;
    [SerializeField, Tooltip("Height while standing")]
    private float standHeight = 2.0f;
    [SerializeField, Tooltip("Speed of transitioning from crouch to stand")]
    private float crouchSpeed = 4.0f;
    private bool isCrouching;

    // Camera and head bobbing settings
    [Header("Camera Settings")]
    [SerializeField, Tooltip("Camera object for head bobbing and rotation effects")]
    private Camera playerCamera;
    [SerializeField, Tooltip("Head bobbing frequency")]
    private float headBobFrequency = 1.5f;
    [SerializeField, Tooltip("Head bobbing amplitude")]
    private float headBobAmplitude = 0.1f;
    private Vector3 cameraStartLocalPosition;
    private float headBobTimer;

    [Header("Size Change Settings")]
    [SerializeField] private float sizeReductionFactor = 10f;
    [SerializeField] private float shrinkDuration = 1f; // Duration of the shrinking process
    private bool isSmall = false;
    private bool isChangingSize = false; // Flag to indicate if currently changing size


    private void Awake()
    {
        // Initialize controls and components
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        cameraStartLocalPosition = playerCamera.transform.localPosition;

        // Store original sizes and physics values
        originalScale = transform.localScale;
        originalColliderHeight = capsuleCollider.height;
        originalColliderRadius = capsuleCollider.radius;
        originalGravityScale = gravityScale;
        originalFallMultiplier = fallMultiplier;
        originalJumpForce = jumpForce;
        originalMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        playerControls.FPSPlayerActions.Enable();
        playerControls.FPSPlayerActions.Jump.performed += OnJumpPerformed;
        playerControls.FPSPlayerActions.ChangeSize.performed += OnSizeChangePerformed;
    }

    private void OnDisable()
    {
        playerControls.FPSPlayerActions.Disable();
        playerControls.FPSPlayerActions.Jump.performed -= OnJumpPerformed;
        playerControls.FPSPlayerActions.ChangeSize.performed -= OnSizeChangePerformed;
    }

    private void Update()
    {
        if (!isChangingSize)
        {
            HandleCrouchInput();
            AlignPlayerWithCameraDirection();
            HandleHeadBobbing();
        }
    }

    private void FixedUpdate()
    {
        if (!isChangingSize)
        {
            Move();
            if (!isGrounded)
            {
                ApplyCustomGravity();
            }


            CheckGrounded();

            if (isGrounded)
            {
                if (wasInAir)
                {
                    wasInAir = false;
                    ResetJumpState(); // Reset jump state when grounded
                }
            }
            else
            {
                wasInAir = true;
            }
        }
    }

    private void HandleCrouchInput()
    {
        if (playerControls.FPSPlayerActions.Crouch.triggered)
        {
            ToggleCrouch();
        }
    }

    private void AlignPlayerWithCameraDirection()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, cameraForward, Time.deltaTime * rotationSpeed);
    }

    private void Move()
    {
        Vector2 inputVector = playerControls.FPSPlayerActions.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y));

        if (isGrounded)
        {
            float currentSpeed = moveSpeed * (playerControls.FPSPlayerActions.Sprint.ReadValue<float>() > 0 ? sprintMultiplier : 1f);
            // Apply movement while preserving y-axis velocity (for gravity/jumping)
            rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
        }
        else
        {
            // Apply air control
            Vector3 airVelocity = moveDirection * moveSpeed * airControl;
            rb.velocity = new Vector3(rb.velocity.x + airVelocity.x, rb.velocity.y, rb.velocity.z + airVelocity.z);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed * sprintMultiplier);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (currentJumpCount < maxJumpCount && !isCrouching && canJump)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        // Reset vertical velocity to zero before applying jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Add jump force
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Update jump state
        isGrounded = false;
        currentJumpCount++;
        lastJumpTime = Time.time;

        if (currentJumpCount >= maxJumpCount)
        {
            canJump = false; // Disable further jumps until reset
        }
    }


    private void ResetJumpState()
    {
        if (isGrounded && Time.time - lastJumpTime >= jumpCooldown)
        {
            currentJumpCount = 0;
            canJump = true;
        }
    }

    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        // Check if player landed during the cooldown
        if (isGrounded)
        {
            canJump = true;
            currentJumpCount = 0;
        }
    }

    private void CheckGrounded()
    {
        Vector3 origin = transform.position + capsuleCollider.center;
        bool rayGrounded = Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, capsuleCollider.height / 2 + groundCheckDistance, groundLayer);

        isGrounded = rayGrounded;
    }

    private void ApplyCustomGravity()
    {
        if (!isGrounded || rb.velocity.y < 0)
        {
            // Apply stronger gravity when falling
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !playerControls.FPSPlayerActions.Jump.IsPressed())
        {
            // Apply normal gravity when ascending and not holding jump
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityScale - 1) * Time.fixedDeltaTime;
        }
    }

    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        StartCoroutine(AdjustCrouchHeight(isCrouching ? crouchHeight : standHeight));
    }

    private IEnumerator AdjustCrouchHeight(float targetHeight)
    {
        float originalHeight = capsuleCollider.height;
        Vector3 originalCenter = capsuleCollider.center;
        float timeElapsed = 0f;

        while (timeElapsed < crouchSpeed)
        {
            // Calculate the current height and center based on the elapsed time
            capsuleCollider.height = Mathf.Lerp(originalHeight, targetHeight, timeElapsed / crouchSpeed);
            capsuleCollider.center = new Vector3(0, capsuleCollider.height / 2, 0);

            // Increment the elapsed time
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final height and center are exactly at the target values
        capsuleCollider.height = targetHeight;
        capsuleCollider.center = new Vector3(0, targetHeight / 2, 0);
    }

    private void OnSizeChangePerformed(InputAction.CallbackContext context)
    {
        if (!isChangingSize)
        {
            StartCoroutine(ChangeSize());
        }
    }

    private IEnumerator ChangeSize()
    {
        isChangingSize = true;

        Vector3 targetScale = isSmall ? Vector3.one : Vector3.one / sizeReductionFactor;
        Vector3 initialScale = transform.localScale;
        Vector3 initialCameraPosition = playerCamera.transform.localPosition;
        Vector3 targetCameraPosition = CalculateTargetCameraPosition(initialCameraPosition);

        CinemachineLook cameraScript = playerCamera.GetComponent<CinemachineLook>();
        if (cameraScript != null)
        {
            // Adjust the camera offset based on the player's current size
            cameraScript.AdjustCameraOffset(isSmall, sizeReductionFactor);
        }

        float elapsedTime = 0;

        while (elapsedTime < shrinkDuration)
        {
            float lerpFactor = elapsedTime / shrinkDuration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpFactor);
            playerCamera.transform.localPosition = Vector3.Lerp(initialCameraPosition, targetCameraPosition, lerpFactor);

            // Adjust physics properties proportionally here if necessary

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        playerCamera.transform.localPosition = targetCameraPosition;
        isSmall = !isSmall;

        // Reset or adjust physics properties to final values here if necessary

        isChangingSize = false;
    }


    private Vector3 CalculateTargetCameraPosition(Vector3 initialCameraPosition)
    {
        // Calculate the target position for the camera based on the player's target scale
        return new Vector3(
            initialCameraPosition.x,
            initialCameraPosition.y / (isSmall ? sizeReductionFactor : 1f),
            initialCameraPosition.z
        );
    }

    private void HandleHeadBobbing()
    {
        if (isGrounded)
        {
            headBobTimer += Time.deltaTime * headBobFrequency;
            Vector3 newCameraPosition = cameraStartLocalPosition + new Vector3(0, Mathf.Sin(headBobTimer) * headBobAmplitude, 0);
            playerCamera.transform.localPosition = newCameraPosition;
        }
        else
        {
            headBobTimer = 0;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraStartLocalPosition, Time.deltaTime * headBobFrequency);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }
}

using Cinemachine;
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
    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera; // Reference to the Cinemachine Virtual Camera
    private Vector3 originalFollowOffset;
    private bool isRotationLocked = false;

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

    private bool isJumping;

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
    public bool toggleCrouch = true; // Toggle between 'hold to crouch' and 'toggle crouch'
    [SerializeField, Tooltip("Height while crouching")]
    private float crouchHeight = 1.0f;
    [SerializeField, Tooltip("Height while standing")]
    private float standHeight = 2.0f;
    [SerializeField, Tooltip("Speed of transitioning from crouch to stand")]
    private float crouchSpeed = 4.0f;
    private bool isCrouching;
    [SerializeField] private float crouchJumpMultiplier = 1.2f; // Multiplier for jump force when crouched

    // Camera and head bobbing settings
    [Header("Camera Settings")]
    [SerializeField, Tooltip("Camera object for head bobbing and rotation effects")]
    private Camera playerCamera;
    [SerializeField, Tooltip("Head bobbing frequency")]
    private float headBobFrequency = 1.5f;
    [SerializeField, Tooltip("Head bobbing amplitude")]
    private float headBobAmplitude = 0.1f;
    private Vector3 cameraStartLocalPosition;

    [Header("Size Change Settings")]
    [SerializeField] private float sizeReductionFactor = 10f;
    [SerializeField] private float shrinkDuration = 1f; // Duration of the shrinking process
    private bool isSmall = false;
    private bool isChangingSize = false; // Flag to indicate if currently changing size

    [Header("Climbing Settings")]
    private bool nearClimbableSurface = false;
    private bool isClimbing = false;
    public float climbingSpeed = 3.0f;
    public float wallJumpForce = 10.0f; // Wall jump force

    private bool isOnTrampoline = false;
    private float trampolineBounceMultiplier = 1.0f;
    private float lastTrampolineTouchTime = 0f;
    private float trampolineJumpThreshold = 0.2f;

    // Add new fields for trampoline bounce logic
    private int bounceCount = 0;
    private const int maxBounceCount = 5; // Maximum number of bounces
    private float bounceDamping = 0.8f; // Reduction factor for each bounce

    private bool attemptHighBounce = false;
    private const float highBounceMultiplier = 2.0f; // Multiplier for the enhanced bounce
    private const float bounceTimingWindow = 0.2f; // Time window for enhanced bounce
    private float lastLandingTime; // Time when the player last landed

    private bool hasWeapon = false;
    public GameObject weaponObject;


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

        isSmall = false;

        // Find and assign the Cinemachine Virtual Camera
        cinemachineCamera = GameObject.Find("FPSVirtualCamera").GetComponent<CinemachineVirtualCamera>();
        if (cinemachineCamera != null)
        {
            var transposer = cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                originalFollowOffset = transposer.m_FollowOffset;
            }
        }
        else
        {
            Debug.LogError("Cinemachine Virtual Camera not found in the scene.");
        }

        isOnTrampoline = false;
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
        HandleClimbingInput();

        if (isClimbing)
        {
            HandleClimbingMovement();
        }
        else
        {
            // Check if the player is facing a climbable surface and pressing forward
            if (nearClimbableSurface && IsFacingClimbableSurface() && playerControls.FPSPlayerActions.Move.ReadValue<Vector2>().y > 0)
            {
                StartClimbing();
            }
            else
            {
                if (!isChangingSize)
                {
                    HandleCrouchInput();
                    AlignPlayerWithCameraDirection();
                }

                if (isGrounded && isJumping)
                {
                    isJumping = false;
                    ResetJumpState();
                }

                // Check for jump input within the timing window
                if (isOnTrampoline && playerControls.FPSPlayerActions.Jump.triggered)
                {
                    float timeSinceLanding = Time.time - lastLandingTime;
                    if (timeSinceLanding <= bounceTimingWindow)
                    {
                        attemptHighBounce = true;
                    }
                    else
                    {
                        PerformTrampolineJump();
                    }
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (!isChangingSize)
        {
            if (!isClimbing)
            {
                Move();
                if (!isGrounded)
                {
                    ApplyCustomGravity();
                }
                CheckGrounded();
            }
        }
    }

    public PlayerControls PlayerControls
    {
        get { return playerControls; }
    }

    private void HandleCrouchInput()
    {
        if (toggleCrouch)
        {
            if (playerControls.FPSPlayerActions.Crouch.triggered)
            {
                ToggleCrouch();
            }
        }
        else
        {
            isCrouching = playerControls.FPSPlayerActions.Crouch.IsPressed();
            AdjustCrouchHeight(isCrouching ? crouchHeight : standHeight);
        }
    }

    public void LockRotation(bool lockRotation)
    {
        isRotationLocked = lockRotation;
    }

    private void AlignPlayerWithCameraDirection()
    {
        if (!isRotationLocked)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, cameraForward, Time.deltaTime * rotationSpeed);
        }
    }

    private void Move()
    {
        Vector2 inputVector = playerControls.FPSPlayerActions.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y));

        if (isGrounded)
        {
            float currentSpeed = moveSpeed * (playerControls.FPSPlayerActions.Sprint.ReadValue<float>() > 0 ? sprintMultiplier : 1f);
            rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
        }
        else
        {
            // Apply air control
            Vector3 airVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            airVelocity = Vector3.ClampMagnitude(airVelocity, moveSpeed * sprintMultiplier);

            // Adjust air velocity based on player input, but prevent reversing direction
            rb.velocity = new Vector3(
                Mathf.Lerp(rb.velocity.x, Mathf.Max(rb.velocity.x, airVelocity.x), airControl * Time.fixedDeltaTime),
                rb.velocity.y,
                Mathf.Lerp(rb.velocity.z, Mathf.Max(rb.velocity.z, airVelocity.z), airControl * Time.fixedDeltaTime)
            );
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isClimbing)
        {
            // Calculate jump direction based on where the player is looking
            Vector3 jumpDirection = Camera.main.transform.forward + Vector3.up;
            float climbJumpForce = wallJumpForce; // Use the wall jump force for climbing jump

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset current Y and Z velocity
            rb.AddForce(jumpDirection.normalized * climbJumpForce, ForceMode.Impulse);
            StopClimbing();
        }
        else if (currentJumpCount < maxJumpCount && canJump && isGrounded)
        {
            PerformJump();
            isJumping = true; // Set flag when jump is performed
        }
        else
        {
            Debug.Log($"Cannot jump: Count={currentJumpCount}, Crouching={isCrouching}, CanJump={canJump}");
        }
    }

    private void PerformJump()
    {
        Vector3 jumpDirection;
        float jumpForceToUse;

        if (isClimbing)
        {
            // Jump away from the wall in the direction the player is facing
            jumpDirection = transform.forward + Vector3.up;
            jumpForceToUse = wallJumpForce; // Use the specific wall jump force
            StopClimbing(); // Stop climbing when the player jumps off
        }
        else
        {
            // Regular vertical jump
            jumpDirection = Vector3.up;
            jumpForceToUse = isCrouching ? jumpForce * crouchJumpMultiplier : jumpForce;
        }

        // Apply the jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(jumpDirection.normalized * jumpForceToUse, ForceMode.Impulse);

        currentJumpCount++;
        lastJumpTime = Time.time;

        if (currentJumpCount >= maxJumpCount)
        {
            canJump = false;
        }
    }

    private void ResetJumpState()
    {
        if (isGrounded)
        {
            currentJumpCount = 0;
            canJump = true;
        }
    }

    private void CheckGrounded()
    {
        if (isClimbing)
            return;  // Skip ground checking when climbing

        Vector3 origin = transform.position + capsuleCollider.center;
        float adjustedRadius = groundCheckRadius * transform.localScale.y;
        float adjustedDistance = groundCheckDistance * transform.localScale.y;

        bool rayGrounded = Physics.SphereCast(origin, adjustedRadius, Vector3.down, out RaycastHit hit, capsuleCollider.height / 2 + adjustedDistance, groundLayer);

        if (rayGrounded && !isGrounded)
        {
            isGrounded = true;
            ResetJumpState();
        }
        else if (!rayGrounded)
        {
            isGrounded = false;
        }
    }

    private void ApplyCustomGravity()
    {
        float scaleMultiplier = transform.localScale.y / originalScale.y; // Scale multiplier based on the player's current Y scale

        if (!isGrounded || rb.velocity.y < 0)
        {
            // Apply stronger gravity when falling
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime * scaleMultiplier;
        }
        else if (rb.velocity.y > 0 && !playerControls.FPSPlayerActions.Jump.IsPressed())
        {
            // Apply normal gravity when ascending and not holding jump
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityScale - 1) * Time.fixedDeltaTime * scaleMultiplier;
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

        var transposer = cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
        float originalFollowOffsetY = transposer.m_FollowOffset.y;

        // Calculate the change in height
        float heightChange = targetHeight - originalHeight;

        // Declare newFollowOffsetY outside of the loop
        float newFollowOffsetY = originalFollowOffsetY;

        float timeElapsed = 0f;
        while (timeElapsed < crouchSpeed)
        {
            capsuleCollider.height = Mathf.Lerp(originalHeight, targetHeight, timeElapsed / crouchSpeed);

            // Adjust the center to keep the bottom of the collider at the same position
            capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y + heightChange / 2 * (timeElapsed / crouchSpeed), originalCenter.z);

            // Calculate and interpolate the camera follow offset
            newFollowOffsetY = isSmall ? (originalFollowOffset.y + heightChange / 2) / sizeReductionFactor : originalFollowOffset.y + heightChange / 2;
            transposer.m_FollowOffset.y = Mathf.Lerp(originalFollowOffsetY, newFollowOffsetY, timeElapsed / crouchSpeed);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        capsuleCollider.height = targetHeight;
        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y + heightChange / 2, originalCenter.z);
        transposer.m_FollowOffset.y = newFollowOffsetY;
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
        isSmall = !isSmall;

        Vector3 targetScale = isSmall ? Vector3.one / sizeReductionFactor : Vector3.one;
        Vector3 initialScale = transform.localScale;

        var transposer = cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 initialFollowOffset = transposer.m_FollowOffset;
        Vector3 targetFollowOffset = initialFollowOffset;

        // Adjust camera follow offset based on new size
        if (isSmall)
        {
            targetFollowOffset.y = originalFollowOffset.y / sizeReductionFactor;
        }
        else
        {
            targetFollowOffset = originalFollowOffset;
        }

        float elapsedTime = 0;
        while (elapsedTime < shrinkDuration)
        {
            float lerpFactor = elapsedTime / shrinkDuration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpFactor);
            transposer.m_FollowOffset = Vector3.Lerp(initialFollowOffset, targetFollowOffset, lerpFactor);
            UpdatePlayerProperties(isSmall ? 1f / sizeReductionFactor : 1f, lerpFactor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        transposer.m_FollowOffset = targetFollowOffset;
        UpdatePlayerProperties(isSmall ? 1f / sizeReductionFactor : 1f);

        isChangingSize = false;

        // Check and adjust crouch height if necessary
        if (isCrouching)
        {
            float newCrouchHeight = crouchHeight / (isSmall ? sizeReductionFactor : 1f);
            StartCoroutine(AdjustCrouchHeight(newCrouchHeight));
        }
    }

    private void UpdatePlayerProperties(float scaleFactor, float lerpFactor = 1f)
    {
        // Adjust jump force relative to the size
        float scaleJumpMultiplier = Mathf.Clamp(scaleFactor, 0.5f, 2f); // Clamp the scale factor for jump force to avoid extreme values
        jumpForce = Mathf.Lerp(jumpForce, originalJumpForce * scaleJumpMultiplier, lerpFactor);

        // Adjust movement speed
        moveSpeed = Mathf.Lerp(moveSpeed, originalMoveSpeed * scaleFactor, lerpFactor);

        // Add other properties if needed
    }

    public void EnableClimbing(bool enable)
    {
        isClimbing = enable;
        rb.useGravity = !enable;
        if (enable)
        {
            rb.velocity = Vector3.zero; // Stop any current movement when starting to climb
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero; // Reset velocity when starting to climb
    }

    private void StopClimbing()
    {
        isClimbing = false;
        rb.useGravity = true;
    }

    private void HandleClimbingInput()
    {
        if (nearClimbableSurface && playerControls.FPSPlayerActions.Interact.triggered)
        {
            if (!isClimbing)
            {
                StartClimbing();
            }
            else
            {
                StopClimbing();
            }
        }
        else if (isClimbing && !nearClimbableSurface)
        {
            StopClimbing();
        }
    }

    private void HandleClimbingMovement()
    {
        if (isClimbing)
        {
            float verticalInput = playerControls.FPSPlayerActions.Move.ReadValue<Vector2>().y;
            float horizontalInput = playerControls.FPSPlayerActions.Move.ReadValue<Vector2>().x;
            Vector3 climbingMovement = new Vector3(horizontalInput, verticalInput, 0) * climbingSpeed;
            rb.MovePosition(rb.position + climbingMovement * Time.deltaTime);
        }
    }

    private bool IsFacingClimbableSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f)) // Adjust distance as needed
        {
            return hit.collider.CompareTag("Wall");
        }
        return false;
    }

    public void OnTrampolineEnter(float multiplier)
    {
        isOnTrampoline = true;
        trampolineBounceMultiplier = multiplier;
        lastTrampolineTouchTime = Time.time;
    }

    public void OnTrampolineExit()
    {
        isOnTrampoline = false;
    }

    private void PerformTrampolineJump()
    {
        float bounceForce;
        if (attemptHighBounce)
        {
            bounceForce = jumpForce * highBounceMultiplier;
            attemptHighBounce = false; // Reset the flag
        }
        else
        {
            bounceForce = jumpForce * trampolineBounceMultiplier;
        }

        rb.velocity = new Vector3(rb.velocity.x, bounceForce, rb.velocity.z);
        isOnTrampoline = false; // Reset the trampoline state
        bounceCount = 0; // Reset the bounce count
    }

    public void ApplyTrampolineBounce(float initialBounceMultiplier)
    {
        if (bounceCount < maxBounceCount)
        {
            float bounceForce = jumpForce * initialBounceMultiplier * Mathf.Pow(bounceDamping, bounceCount);
            rb.velocity = new Vector3(rb.velocity.x, bounceForce, rb.velocity.z);
            bounceCount++;
        }
        else
        {
            // Reset the bounce count and stop bouncing
            bounceCount = 0;
            isOnTrampoline = false;
        }
    }

    public void EnableWeapon()
    {
        if (weaponObject != null)
        {
            // Find the "Model" child object and enable it
            Transform modelTransform = weaponObject.transform;
            if (modelTransform != null)
            {
                modelTransform.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Model not found in weaponObject");
            }
        }
        else
        {
            Debug.LogWarning("Weapon object not assigned in Player");
        }
        // Additional logic if needed
    }

    public bool HasWeapon()
    {
        return hasWeapon;
    }

    public bool CanInteract()
    {
        return true; 
    }

    private void TransitionToTopOfSurface()
    {
        StopClimbing();

        // Additional logic to position the player correctly on the surface
        // This might involve setting the player's position and rotation
        // Example: transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = true;
            Debug.Log("OnCollisionEnter - Grounded");
            isOnTrampoline = false;
        }

        if (collision.gameObject.CompareTag("Trampoline"))
        {
            lastLandingTime = Time.time; // Record the landing time
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
            Debug.Log("OnCollisionExit - Not Grounded");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ClimbTopTrigger") && isClimbing)
        {
            TransitionToTopOfSurface();
        }

        if (other.CompareTag("Wall"))
        {
            nearClimbableSurface = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            nearClimbableSurface = false;
            if (isClimbing)
            {
                StopClimbing();
            }
        }
    }
}

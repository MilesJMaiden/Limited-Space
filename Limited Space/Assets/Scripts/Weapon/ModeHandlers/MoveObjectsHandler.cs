using UnityEngine;
using UnityEngine.InputSystem;

public class MoveObjectsHandler
{
    private CinemachineLook cinemachineLook;
    private AdvancedPlayerMovement playerMovement;

    private AdvancedArmCannon armCannon;
    private GameObject heldObject;
    private float heldObjectDistance;
    private Rigidbody heldObjectRigidbody; // Store Rigidbody of held object
    private float moveSpeed;
    private float rotateSpeed;
    private bool isRotatingObject = false;

    private Quaternion savedPlayerRotation;

    public MoveObjectsHandler(AdvancedArmCannon armCannon, float moveSpeed, float rotateSpeed, CinemachineLook cinemachineLook, AdvancedPlayerMovement playerMovement)
    {
        this.armCannon = armCannon;
        this.moveSpeed = moveSpeed;
        this.rotateSpeed = rotateSpeed;
        this.cinemachineLook = cinemachineLook;
        this.playerMovement = playerMovement;
    }

    public void TryGrabObject()
    {
        if (heldObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(armCannon.transform.position, armCannon.transform.forward, out hit))
            {
                MovableObject movableObject = hit.collider.GetComponent<MovableObject>();
                if (movableObject != null)
                {
                    heldObject = hit.collider.gameObject;
                    heldObjectDistance = Vector3.Distance(armCannon.transform.position, heldObject.transform.position);
                    heldObjectRigidbody = heldObject.GetComponent<Rigidbody>();

                    if (heldObjectRigidbody != null)
                    {
                        heldObjectRigidbody.useGravity = false;
                        heldObjectRigidbody.velocity = Vector3.zero; // Stop any existing movement
                        heldObjectRigidbody.angularVelocity = Vector3.zero; // Stop any existing rotation
                    }

                    movableObject.Grab();
                }
            }
        }
        else
        {
            ReleaseHeldObject();
        }
    }

    private void ReleaseHeldObject()
    {
        if (heldObject != null)
        {
            MovableObject movableObject = heldObject.GetComponent<MovableObject>();
            if (movableObject != null)
            {
                movableObject.Release();
            }

            if (heldObjectRigidbody != null)
            {
                heldObjectRigidbody.useGravity = true;
            }

            heldObject = null;
            heldObjectRigidbody = null;
        }
    }

    public void MoveObject(Vector2 movement)
    {
        if (heldObject != null && !isRotatingObject)
        {
            heldObjectDistance += movement.y * moveSpeed * Time.deltaTime;
            heldObjectDistance = Mathf.Clamp(heldObjectDistance, 1.0f, 10.0f);

            Vector3 newPosition = armCannon.transform.position + armCannon.transform.forward * heldObjectDistance;
            heldObject.transform.position = newPosition;
        }
    }

    public void RotateObject(Vector2 rotation)
    {
        if (heldObject != null && isRotatingObject)
        {
            float xRotation = rotation.x * rotateSpeed * Time.deltaTime;
            float yRotation = rotation.y * rotateSpeed * Time.deltaTime;
            heldObject.transform.Rotate(armCannon.transform.up, xRotation, Space.World);
            heldObject.transform.Rotate(armCannon.transform.right, yRotation, Space.World);
        }
    }

    public void EnterObjectRotationMode()
    {
        // Save current player rotation and lock camera and player rotation
        savedPlayerRotation = playerMovement.transform.rotation;
        cinemachineLook.SetCameraMovementEnabled(false);
        playerMovement.LockRotation(true);

        isRotatingObject = true;
    }

    public void ExitObjectRotationMode()
    {
        // Restore player rotation and unlock camera and player rotation
        playerMovement.transform.rotation = savedPlayerRotation;
        cinemachineLook.SetCameraMovementEnabled(true);
        playerMovement.LockRotation(false);

        isRotatingObject = false;
    }

    public void Update()
    {
        if (heldObject != null)
        {
            Vector3 newPosition = armCannon.transform.position + armCannon.transform.forward * heldObjectDistance;
            heldObject.transform.position = newPosition;

            // Toggle object rotation mode based on right mouse button
            if (Mouse.current.rightButton.isPressed && !isRotatingObject)
            {
                EnterObjectRotationMode();
            }
            else if (Mouse.current.rightButton.wasReleasedThisFrame && isRotatingObject)
            {
                ExitObjectRotationMode();
            }

            if (isRotatingObject)
            {
                Vector2 rotationDelta = Mouse.current.delta.ReadValue();
                RotateObject(rotationDelta);
            }
        }
    }
}

using System.Collections.Generic;
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

    private List<GameObject> releasedObjects = new List<GameObject>();
    private int maxStoredObjects = 3;
    private Color queuedObjectColor = Color.blue; // Color for objects in the queue

    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

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

                    // Check if the object is already in the released objects list
                    if (releasedObjects.Contains(heldObject))
                    {
                        releasedObjects.Remove(heldObject);
                        ResetObjectColor(heldObject);
                    }
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
                ManageReleasedObjects(heldObject);
            }

            heldObject = null;
            heldObjectRigidbody = null;
        }
    }

    private void ManageReleasedObjects(GameObject releasedObject)
    {
        // Add the object to the released objects list
        if (!releasedObjects.Contains(releasedObject))
        {
            releasedObjects.Add(releasedObject);
            ChangeObjectColor(releasedObject, queuedObjectColor);
            DisablePhysics(releasedObject);
        }

        // Remove the oldest object if the list exceeds the maximum size
        if (releasedObjects.Count > maxStoredObjects)
        {
            GameObject oldestObject = releasedObjects[0];
            releasedObjects.RemoveAt(0);
            ResetObjectColor(oldestObject);
            EnablePhysics(oldestObject);
            ResetObjectState(oldestObject);
        }
    }

    private void ResetObjectState(GameObject obj)
    {
        // Re-enable gravity and remove all constraints from the object's Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;

            // Reset velocity and angular velocity to stop any residual movement
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void DisablePhysics(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void EnablePhysics(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
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

    private void ChangeObjectColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalColors[obj] = renderer.material.color;
            renderer.material.color = color;
        }
    }

    private void ResetObjectColor(GameObject obj)
    {
        if (originalColors.TryGetValue(obj, out Color originalColor))
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = originalColor;
            }
        }
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

using UnityEngine;

public class MoveObjectsHandler
{
    private AdvancedArmCannon armCannon;
    private GameObject heldObject;
    private float heldObjectDistance;

    public MoveObjectsHandler(AdvancedArmCannon armCannon)
    {
        this.armCannon = armCannon;
    }

    public void TryGrabObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(armCannon.transform.position, armCannon.transform.forward, out hit))
        {
            // Check if the hit object has the tag to be moved
            if (hit.collider.CompareTag("Movable"))
            {
                heldObject = hit.collider.gameObject;
                heldObjectDistance = Vector3.Distance(armCannon.transform.position, heldObject.transform.position);
                // Additional setup like disabling rigidbody gravity, etc.
            }
        }
    }

    public void MoveObject(Vector2 movement)
    {
        if (heldObject != null)
        {
            heldObjectDistance += movement.y * armCannon.moveObjectSpeed * Time.deltaTime;
            Vector3 newPosition = armCannon.transform.position + armCannon.transform.forward * heldObjectDistance;
            heldObject.transform.position = newPosition;
        }
    }

    public void RotateObject(float rotation)
    {
        if (heldObject != null)
        {
            heldObject.transform.Rotate(Vector3.up, rotation * armCannon.rotateSpeed * Time.deltaTime);
        }
    }

    public void Update()
    {
        // Implement logic for continuous object holding, like raycasting to keep the object at a distance
    }
}

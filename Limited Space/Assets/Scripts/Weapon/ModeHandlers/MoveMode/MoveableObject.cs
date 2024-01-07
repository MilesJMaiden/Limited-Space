using UnityEngine;

public class MovableObject : MonoBehaviour
{
    private Rigidbody rb;

    public bool IsBeingMoved { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("MovableObject requires a Rigidbody component", this);
        }
    }

    public void Grab()
    {
        IsBeingMoved = true;

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Additional logic when the object is grabbed (e.g., visual effects, sound effects)
    }

    public void Release()
    {
        // When the object is released by the arm cannon
        IsBeingMoved = false;

        // Re-enable gravity and unfreeze rotation
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        // Additional logic when the object is released (e.g., visual effects, sound effects)
    }
}

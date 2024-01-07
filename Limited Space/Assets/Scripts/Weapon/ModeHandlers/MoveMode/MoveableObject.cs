using UnityEngine;

public class MovableObject : MonoBehaviour
{
    private Rigidbody rb;

    // Indicates whether the object is currently being moved by the arm cannon
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
        // When the object is grabbed by the arm cannon
        IsBeingMoved = true;

        // Optional: Turn off gravity and freeze rotation for smoother movement
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

    // Optional: Implement additional methods for specific behaviors
}

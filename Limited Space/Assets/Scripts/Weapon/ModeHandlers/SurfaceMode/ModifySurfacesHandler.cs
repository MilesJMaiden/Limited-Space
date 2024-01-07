using System.Collections.Generic;
using UnityEngine;

public class ModifySurfacesHandler
{
    private AdvancedArmCannon armCannon;
    private Queue<GameObject> modifiedSurfaces;
    private Dictionary<GameObject, Color> originalColors;
    private const int MaxActiveSurfaces = 3; // Max number of active surfaces
    public Color wallModifiedColor = Color.red;
    public Color floorModifiedColor = Color.green;

    public ModifySurfacesHandler(AdvancedArmCannon armCannon, int maxModifiedSurfaces)
    {
        this.armCannon = armCannon;
        modifiedSurfaces = new Queue<GameObject>();
        originalColors = new Dictionary<GameObject, Color>();
    }

    public void ModifySurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(armCannon.transform.position, armCannon.transform.forward, out hit))
        {
            GameObject surface = hit.collider.gameObject;

            // Check if the surface is already in the queue
            if (!modifiedSurfaces.Contains(surface))
            {
                if (!originalColors.ContainsKey(surface))
                {
                    Renderer surfaceRenderer = surface.GetComponent<Renderer>();
                    originalColors[surface] = surfaceRenderer.material.color;

                    // Determine type of surface and modify
                    if (IsWall(surface))
                    {
                        surfaceRenderer.material.color = wallModifiedColor;
                        AddClimbingTrigger(surface);
                    }
                    else if (IsFloor(surface))
                    {
                        surfaceRenderer.material.color = floorModifiedColor;
                        MakeTrampoline(surface);
                    }
                }

                // If the queue is full, reset the oldest surface
                if (modifiedSurfaces.Count >= MaxActiveSurfaces)
                {
                    GameObject oldSurface = modifiedSurfaces.Dequeue();
                    ResetSurfaceBehavior(oldSurface);
                }

                modifiedSurfaces.Enqueue(surface);
            }
        }
    }

    private void AddClimbingTrigger(GameObject surface)
    {
        // Add a trigger collider for climbing
        if (surface.GetComponent<ClimbableSurface>() == null)
        {
            ClimbableSurface climbable = surface.AddComponent<ClimbableSurface>();
            BoxCollider triggerCollider = surface.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = new Vector3(1.025f, 0.995f, 1.025f); // Adjust size as needed
            climbable.SetTriggerCollider(triggerCollider);
        }
    }

    private bool IsWall(GameObject surface)
    {
        return surface.CompareTag("Wall");
    }

    private bool IsFloor(GameObject surface)
    {
        return surface.CompareTag("Ground");
    }

    private void MakeTrampoline(GameObject surface)
    {
        if (surface.GetComponent<TrampolineSurface>() == null)
        {
            surface.AddComponent<TrampolineSurface>();
        }
    }

    private void ResetSurfaceBehavior(GameObject surface)
    {
        if (surface)
        {
            ResetSurfaceColor(surface);

            // Remove the ClimbableSurface component and its trigger collider
            var climbable = surface.GetComponent<ClimbableSurface>();
            if (climbable != null)
            {
                // Find and destroy the trigger collider added for climbing
                Collider[] colliders = surface.GetComponents<Collider>();
                foreach (var collider in colliders)
                {
                    if (collider.isTrigger)
                    {
                        UnityEngine.Object.Destroy(collider);
                        break; // Assuming there is only one trigger collider for climbing
                    }
                }

                UnityEngine.Object.Destroy(climbable);
            }

            // Similar logic for TrampolineSurface, if applicable
            var trampoline = surface.GetComponent<TrampolineSurface>();
            if (trampoline != null)
            {
                // Remove any specific components or settings related to trampoline
                UnityEngine.Object.Destroy(trampoline);
            }
        }
    }

    private void ResetSurfaceColor(GameObject surface)
    {
        if (originalColors.TryGetValue(surface, out Color originalColor))
        {
            Renderer surfaceRenderer = surface.GetComponent<Renderer>();
            if (surfaceRenderer != null)
            {
                surfaceRenderer.material.color = originalColor;
                originalColors.Remove(surface);
            }
        }
    }

    public void Update() { }
}

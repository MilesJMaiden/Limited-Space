using System.Collections.Generic;
using UnityEngine;

public class ModifySurfacesHandler
{
    private AdvancedArmCannon armCannon;
    private Queue<GameObject> modifiedSurfaces;
    private Dictionary<GameObject, Color> originalColors;
    private int maxModifiedSurfaces;
    public Color wallModifiedColor = Color.red; // Set this color in the Inspector for walls
    public Color floorModifiedColor = Color.green; // Set this color in the Inspector for floors

    public ModifySurfacesHandler(AdvancedArmCannon armCannon, int maxModifiedSurfaces)
    {
        this.armCannon = armCannon;
        this.maxModifiedSurfaces = maxModifiedSurfaces;
        modifiedSurfaces = new Queue<GameObject>();
        originalColors = new Dictionary<GameObject, Color>();
    }

    public void ModifySurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(armCannon.transform.position, armCannon.transform.forward, out hit))
        {
            GameObject surface = hit.collider.gameObject;
            if (!originalColors.ContainsKey(surface))
            {
                Renderer surfaceRenderer = surface.GetComponent<Renderer>();
                originalColors[surface] = surfaceRenderer.material.color;

                // Determine if it's a wall or a floor and modify accordingly
                if (IsWall(surface))
                {
                    surfaceRenderer.material.color = wallModifiedColor;
                    MakeClimbable(surface);
                }
                else if (IsFloor(surface))
                {
                    surfaceRenderer.material.color = floorModifiedColor;
                    MakeTrampoline(surface);
                }
            }

            if (modifiedSurfaces.Count >= maxModifiedSurfaces)
            {
                GameObject oldSurface = modifiedSurfaces.Dequeue();
                ResetSurfaceBehavior(oldSurface);
            }

            modifiedSurfaces.Enqueue(surface);
        }
    }

    private bool IsWall(GameObject surface)
    {
        // Implement logic to determine if the surface is a wall
        // Example: Check the surface's tag or layer
        return surface.CompareTag("Wall");
    }

    private bool IsFloor(GameObject surface)
    {
        // Implement logic to determine if the surface is a floor
        return surface.CompareTag("Floor");
    }

    private void MakeClimbable(GameObject surface)
    {
        if (surface.GetComponent<ClimbableSurface>() == null)
        {
            surface.AddComponent<ClimbableSurface>();
        }
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

            var climbable = surface.GetComponent<ClimbableSurface>();
            if (climbable != null)
            {
                UnityEngine.Object.Destroy(climbable);
            }

            var trampoline = surface.GetComponent<TrampolineSurface>();
            if (trampoline != null)
            {
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

    public void Update()
    {
        // Additional update logic if needed
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ModifySurfacesHandler
{
    private AdvancedArmCannon armCannon;
    private Queue<GameObject> modifiedSurfaces;
    private Dictionary<GameObject, Color> originalColors;
    private int maxModifiedSurfaces;
    public Color modifiedColor; // Set this color in the Inspector

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
            // Check if the surface is already modified
            if (!originalColors.ContainsKey(surface))
            {
                // Save original color
                Renderer surfaceRenderer = surface.GetComponent<Renderer>();
                originalColors[surface] = surfaceRenderer.material.color;

                // Change color to indicate modification
                surfaceRenderer.material.color = modifiedColor;

                // Implement other logic to modify surface properties
            }

            // Manage the queue of modified surfaces
            if (modifiedSurfaces.Count >= maxModifiedSurfaces)
            {
                GameObject oldSurface = modifiedSurfaces.Dequeue();
                ResetSurfaceColor(oldSurface);
            }

            modifiedSurfaces.Enqueue(surface);
        }
    }

    private void ResetSurfaceColor(GameObject surface)
    {
        if (surface && originalColors.TryGetValue(surface, out Color originalColor))
        {
            Renderer surfaceRenderer = surface.GetComponent<Renderer>();
            surfaceRenderer.material.color = originalColor;
            originalColors.Remove(surface);
        }
    }

    public void Update()
    {
        // Additional update logic if needed
    }
}

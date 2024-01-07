using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private HashSet<Keycard.KeycardType> collectedKeycards = new HashSet<Keycard.KeycardType>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddKeycard(Keycard.KeycardType keycardType)
    {
        collectedKeycards.Add(keycardType);
        HUDManager.Instance.UpdateKeycardUI(keycardType, true);
    }

    public bool HasKeycard(Keycard.KeycardType keycardType)
    {
        return collectedKeycards.Contains(keycardType);
    }
}

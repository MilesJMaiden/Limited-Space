using UnityEngine;
using TMPro;
using System.Collections;

public class Door : MonoBehaviour
{
    public Keycard.KeycardType requiredKeycard;
    public TextMeshProUGUI worldSpaceText; // Assign in Inspector
    public float openHeight = 3f; // Height to which the door opens
    public float openSpeed = 1f; // Speed at which the door opens

    private bool isPlayerNear = false;
    private bool isDoorOpen = false;

    private void Start()
    {
        if (worldSpaceText != null)
            worldSpaceText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            UpdateWorldSpaceText(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (worldSpaceText != null)
                worldSpaceText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (InventorySystem.Instance.HasKeycard(requiredKeycard) && !isDoorOpen)
            {
                OpenDoor();
                if (worldSpaceText != null)
                    Destroy(worldSpaceText.gameObject); // Destroy text when door is opened
            }
        }
    }

    private void UpdateWorldSpaceText(Collider player)
    {
        if (worldSpaceText != null)
        {
            worldSpaceText.gameObject.SetActive(true);
            if (InventorySystem.Instance.HasKeycard(requiredKeycard))
                worldSpaceText.text = "Press 'E' to Open";
            else
                worldSpaceText.text = "You require a key";
        }
    }

    private void OpenDoor()
    {
        isDoorOpen = true;
        StartCoroutine(AnimateDoorOpen());
    }

    private IEnumerator AnimateDoorOpen()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * openHeight;

        float elapsedTime = 0;
        while (elapsedTime < openSpeed)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / openSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // Ensure door reaches the final position
    }
}

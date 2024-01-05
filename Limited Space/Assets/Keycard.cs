using UnityEngine;

public class Keycard : MonoBehaviour
{
    public enum KeycardType { Red, Green, Blue, Gold }
    public KeycardType keycardType;
    public GameObject pickUpUI; // World space UI
    public AudioSource audioSourcePrefab; // Prefab with AudioSource component
    public GameObject vfxPrefab; // Prefab for VFX

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUpUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pickUpUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (pickUpUI.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            CollectKeycard();
        }
    }

    private void CollectKeycard()
    {
        Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
        Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        InventorySystem.Instance.AddKeycard(keycardType);
        Destroy(gameObject);
    }
}

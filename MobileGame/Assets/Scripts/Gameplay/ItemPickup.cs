using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerItemSystem.ItemType itemType;
    [SerializeField] private GameObject itemVisualPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerItemSystem itemSystem = other.GetComponent<PlayerItemSystem>();
        if (itemSystem != null)
        {
            itemSystem.PickupItem(itemType, itemVisualPrefab);
            Destroy(gameObject);
        }
    }
}
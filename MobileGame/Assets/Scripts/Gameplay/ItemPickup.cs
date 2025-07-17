using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerItemSystem.ItemType itemType;
    [SerializeField] private GameObject itemVisualPrefab;

    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp)
        {
            Debug.Log("Already picked up, ignoring trigger.");
            return; // prevent multiple pickups
        }

        PlayerItemSystem itemSystem = other.GetComponent<PlayerItemSystem>();
        if (itemSystem != null)
        {
            Vector3 pickupPosition = transform.position;
            Debug.Log($"Player detected for item pickup: {itemType} at {pickupPosition}");

            itemSystem.PickupItem(itemType, itemVisualPrefab, pickupPosition);

            pickedUp = true;
            Debug.Log($"Picked up {itemType}, destroying pickup object.");
            gameObject.SetActive(false);
        }
    }

    public void ResetPickup()
    {
        pickedUp = false;
    }

}
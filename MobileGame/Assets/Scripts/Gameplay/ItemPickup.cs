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
            return;
        }

        PlayerItemSystem itemSystem = other.GetComponent<PlayerItemSystem>();
        if (itemSystem != null)
        {
            Vector3 pickupPosition = transform.position;

            itemSystem.PickupItem(itemType, itemVisualPrefab, pickupPosition);

            pickedUp = true;
            gameObject.SetActive(false);
        }
    }

    public void ResetPickup()
    {
        pickedUp = false;
    }

}
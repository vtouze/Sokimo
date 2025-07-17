using System.Collections;
using UnityEngine;

public class PlayerItemSystem : MonoBehaviour
{
    public enum ItemType { None, Key, Sword }

    [SerializeField] private Transform keyHolder;
    [SerializeField] private Transform swordHolder;

    private GameObject currentItemVisual;
    private ItemType currentItem = ItemType.None;
    private GameObject currentItemPrefab;

    public bool HasKey => currentItem == ItemType.Key;
    public bool HasSword => currentItem == ItemType.Sword;

    public void PickupItem(ItemType newItem, GameObject newVisualPrefab, Vector3 pickupPosition)
    {
        Debug.Log($"Player tries to pick up {newItem} at position {pickupPosition}");

        if (currentItem != ItemType.None && currentItemPrefab != null)
        {
            GameObject dropped = Instantiate(currentItemPrefab, pickupPosition, Quaternion.identity);
            dropped.SetActive(true);
            SetupDroppedItemCollider(dropped);

            ItemPickup itemPickup = dropped.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.ResetPickup();
            }

            Debug.Log($"Dropped {currentItem} at {pickupPosition}");
        }
        else
        {
            Debug.Log("No item to drop.");
        }

        // Destroy old visual
        if (currentItemVisual != null)
        {
            Debug.Log($"Destroying current visual for {currentItem}");
            Destroy(currentItemVisual);
            currentItemVisual = null;
        }

        currentItem = newItem;
        currentItemPrefab = newVisualPrefab;

        Debug.Log($"Now holding {currentItem}");

        if (keyHolder != null) keyHolder.gameObject.SetActive(false);
        if (swordHolder != null) swordHolder.gameObject.SetActive(false);

        if (newVisualPrefab != null)
        {
            Transform targetHolder = newItem switch
            {
                ItemType.Key => keyHolder,
                ItemType.Sword => swordHolder,
                _ => null
            };

            if (targetHolder != null)
            {
                targetHolder.gameObject.SetActive(true);
                currentItemVisual = Instantiate(newVisualPrefab, targetHolder.position, Quaternion.identity, targetHolder);
                Debug.Log($"Instantiated visual for {newItem} on {targetHolder.name}");
            }
        }
    }

    private void SetupDroppedItemCollider(GameObject dropped)
    {
        Collider2D col = dropped.GetComponent<Collider2D>();
        if (col != null)
        {
            Debug.Log($"Setting collider on dropped item {dropped.name}: isTrigger = false, temporarily disabling collider");
            col.isTrigger = false;

            StartCoroutine(ReenableColliderAfterDelay(col, 3));
        }
    }

    private IEnumerator ReenableColliderAfterDelay(Collider2D col, float delay)
    {
        col.enabled = false;
        yield return new WaitForSeconds(delay);
        col.enabled = true;
        Debug.Log($"Re-enabled collider on dropped item after {delay} seconds");
    }

    public void ConsumeKey()
    {
        if (currentItem == ItemType.Key)
        {
            Debug.Log("Consuming key");
            ClearItem();
        }
    }

    public void ConsumeSword()
    {
        if (currentItem == ItemType.Sword)
        {
            Debug.Log("Consuming sword");
            ClearItem();
        }
    }

    private void ClearItem()
    {
        Debug.Log($"Clearing held item {currentItem}");
        currentItem = ItemType.None;
        currentItemPrefab = null;

        if (currentItemVisual != null)
        {
            Destroy(currentItemVisual);
            currentItemVisual = null;
        }

        if (keyHolder != null) keyHolder.gameObject.SetActive(false);
        if (swordHolder != null) swordHolder.gameObject.SetActive(false);
    }
}
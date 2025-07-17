using System.Collections;
using UnityEngine;

public class PlayerItemSystem : MonoBehaviour
{
    public enum ItemType { None, Key, Sword }

    [SerializeField] private Transform keyHolder;
    [SerializeField] private Transform swordHolder;

    private GameObject currentItemVisual;
    private ItemType currentItem = ItemType.None;
    private GameObject currentItemPrefab; // Original prefab for dropping

    public bool HasKey => currentItem == ItemType.Key;
    public bool HasSword => currentItem == ItemType.Sword;

    public void PickupItem(ItemType newItem, GameObject newVisualPrefab, Vector3 pickupPosition)
    {
        Debug.Log($"Player tries to pick up {newItem} at position {pickupPosition}");

        if (currentItem != ItemType.None && currentItemPrefab != null)
        {
            GameObject dropped = Instantiate(currentItemPrefab, pickupPosition, Quaternion.identity);
            SetupDroppedItemCollider(dropped);

            // 🔁 Reset pickup state on the newly dropped object
            ItemPickup itemPickup = dropped.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.ResetPickup(); // ← this is the key!
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
            //currentItemVisual.SetActive(false);
            Destroy(currentItemVisual);
            currentItemVisual = null;
        }

        currentItem = newItem;
        currentItemPrefab = newVisualPrefab;

        Debug.Log($"Now holding {currentItem}");

        // Disable holders initially
        if (keyHolder != null) keyHolder.gameObject.SetActive(false);
        if (swordHolder != null) swordHolder.gameObject.SetActive(false);

        // Instantiate new visual on correct holder
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
            else
            {
                Debug.LogWarning("No valid holder found for item type " + newItem);
            }
        }
        else
        {
            Debug.LogWarning("New visual prefab is null for item " + newItem);
        }
    }

    private void SetupDroppedItemCollider(GameObject dropped)
    {
        Collider2D col = dropped.GetComponent<Collider2D>();
        if (col != null)
        {
            Debug.Log($"Setting collider on dropped item {dropped.name}: isTrigger = false, temporarily disabling collider");
            col.isTrigger = false;

            // Disable collider briefly to avoid immediate pickup issues
            StartCoroutine(ReenableColliderAfterDelay(col, 3));
        }
        else
        {
            Debug.LogWarning($"Dropped item {dropped.name} has no Collider2D");
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
        else
        {
            Debug.LogWarning("Attempted to consume key but no key is held");
        }
    }

    public void ConsumeSword()
    {
        if (currentItem == ItemType.Sword)
        {
            Debug.Log("Consuming sword");
            ClearItem();
        }
        else
        {
            Debug.LogWarning("Attempted to consume sword but no sword is held");
        }
    }

    private void ClearItem()
    {
        Debug.Log($"Clearing held item {currentItem}");
        currentItem = ItemType.None;
        currentItemPrefab = null;

        if (currentItemVisual != null)
        {
            //currentItemVisual.SetActive(false);
            Destroy(currentItemVisual);
            currentItemVisual = null;
        }

        if (keyHolder != null) keyHolder.gameObject.SetActive(false);
        if (swordHolder != null) swordHolder.gameObject.SetActive(false);
    }
}
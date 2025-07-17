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
        }

        if (currentItemVisual != null)
        {
            Destroy(currentItemVisual);
            currentItemVisual = null;
        }

        currentItem = newItem;
        currentItemPrefab = newVisualPrefab;

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
            }
        }
    }

    private void SetupDroppedItemCollider(GameObject dropped)
    {
        Collider2D col = dropped.GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = false;

            StartCoroutine(ReenableColliderAfterDelay(col, 3));
        }
    }

    private IEnumerator ReenableColliderAfterDelay(Collider2D col, float delay)
    {
        col.enabled = false;
        yield return new WaitForSeconds(delay);
        col.enabled = true;
    }

    public void ConsumeKey()
    {
        if (currentItem == ItemType.Key)
        {
            ClearItem();
        }
    }

    public void ConsumeSword()
    {
        if (currentItem == ItemType.Sword)
        {
            ClearItem();
        }
    }

    private void ClearItem()
    {
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
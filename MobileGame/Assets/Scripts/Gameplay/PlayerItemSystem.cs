using UnityEngine;

public class PlayerItemSystem : MonoBehaviour
{
    public enum ItemType { None, Key }

    [SerializeField] private Transform itemHolder;
    private GameObject currentItemVisual;
    private ItemType currentItem = ItemType.None;

    public bool HasKey => currentItem == ItemType.Key;

    public void PickupItem(ItemType newItem, GameObject visualPrefab)
    {
        if (currentItemVisual != null)
            Destroy(currentItemVisual);

        currentItem = newItem;

        if (itemHolder != null)
            itemHolder.gameObject.SetActive(true);

        if (visualPrefab != null)
        {
            currentItemVisual = Instantiate(visualPrefab, itemHolder.position, Quaternion.identity, itemHolder);
        }
    }

    public void ConsumeKey()
    {
        if (currentItem == ItemType.Key)
        {
            currentItem = ItemType.None;

            if (currentItemVisual != null)
                Destroy(currentItemVisual);

            if (itemHolder != null)
                itemHolder.gameObject.SetActive(false);
        }
    }
}
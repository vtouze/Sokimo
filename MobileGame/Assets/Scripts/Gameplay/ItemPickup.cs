using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerItemSystem.ItemType itemType;
    [SerializeField] private GameObject itemVisualPrefab;

    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;

        PlayerItemSystem itemSystem = other.GetComponent<PlayerItemSystem>();
        if (itemSystem != null)
        {
            pickedUp = true;
            itemSystem.AnimatePickup(gameObject, itemType, itemVisualPrefab);
        }
    }

    public void ResetPickup()
    {
        pickedUp = false;
    }

}
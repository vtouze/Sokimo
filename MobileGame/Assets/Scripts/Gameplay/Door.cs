using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour
{
    private bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen) return;

        PlayerItemSystem itemSystem = other.GetComponent<PlayerItemSystem>();
        if (itemSystem != null && itemSystem.HasKey)
        {
            itemSystem.ConsumeKey();
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        Destroy(gameObject);
    }
}
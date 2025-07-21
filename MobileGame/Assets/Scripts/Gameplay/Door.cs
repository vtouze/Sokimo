using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsLocked = true;

    public void OpenDoor()
    {
        if (!IsLocked) return;
        IsLocked = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerController.BlockMovement(true);

        LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack);

        LeanTween.delayedCall(0.55f, () =>
        {
            Destroy(gameObject);

            if (playerController != null)
                playerController.BlockMovement(false);
        });
    }
}
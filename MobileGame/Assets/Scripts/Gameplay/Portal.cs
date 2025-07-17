using UnityEngine;

public class Portal : MonoBehaviour
{
    private static Portal portalA;
    private static Portal portalB;

    [SerializeField] private PlayerController playerController;

    // Track which portal the player came from
    private static Portal lastPortalUsed;

    private void Awake()
    {
        if (portalA == null)
        {
            portalA = this;
        }
        else if (portalB == null)
        {
            portalB = this;
        }
    }

    private void OnDestroy()
    {
        if (portalA == this) portalA = null;
        if (portalB == this) portalB = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerController == null) return;
        if (other.gameObject != playerController.gameObject) return;
        if (portalA == null || portalB == null) return;
        if (lastPortalUsed == this) return;

        if (this == portalA)
        {
            Vector3Int targetGridPos = playerController.groundTilemap.WorldToCell(portalB.transform.position);
            playerController.PlayPortalAnimation(targetGridPos);
            lastPortalUsed = portalB;
        }
        else if (this == portalB)
        {
            Vector3Int targetGridPos = playerController.groundTilemap.WorldToCell(portalA.transform.position);
            playerController.PlayPortalAnimation(targetGridPos);
            lastPortalUsed = portalA;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerController == null) return;
        if (other.gameObject != playerController.gameObject) return;

        // Clear last portal used only when the player exits a portal
        if (lastPortalUsed == this)
        {
            lastPortalUsed = null;
        }
    }
}
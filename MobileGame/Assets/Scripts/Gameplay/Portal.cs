using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour
{
    [Header("Linked Portal (Set manually in Inspector)")]
    [SerializeField] private Portal linkedPortal;

    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;

    private static Portal lastPortalUsed;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerController == null) return;
        if (other.gameObject != playerController.gameObject) return;
        if (linkedPortal == null) return;
        if (lastPortalUsed == this) return;

        Vector3Int targetGridPos = playerController.groundTilemap.WorldToCell(linkedPortal.transform.position);
        playerController.PlayPortalAnimation(targetGridPos);

        lastPortalUsed = linkedPortal;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerController == null) return;
        if (other.gameObject != playerController.gameObject) return;

        if (lastPortalUsed == this)
        {
            lastPortalUsed = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (linkedPortal != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, linkedPortal.transform.position);
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }
#endif
}
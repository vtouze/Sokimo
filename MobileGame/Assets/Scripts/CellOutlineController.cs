using UnityEngine;
using UnityEngine.Tilemaps;

public class CellOutlineController : MonoBehaviour
{
    public Tilemap groundTilemap;     // Reference to your ground tilemap
    public PlayerController player;   // Reference to your player controller

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3Int playerCell = player.GetCurrentGridPosition();
        Vector3 cellCenter = groundTilemap.GetCellCenterWorld(playerCell);

        // Position the outline sprite exactly over the current cell
        transform.position = cellCenter;

        // Make sure the outline is rendered *in front* of the ground tilemap
        // by setting the sorting order or z-position:
        spriteRenderer.sortingOrder = 10; // higher than the tilemap's order
    }
}
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SnapToGrid : MonoBehaviour
{
    public Grid grid;

    [ContextMenu("Snap to Grid")]
    void Snap()
    {
        Vector3Int cellPosition = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPosition);
    }
}
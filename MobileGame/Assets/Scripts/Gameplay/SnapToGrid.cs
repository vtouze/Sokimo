using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SnapToGrid : MonoBehaviour
{
    public Grid grid;

    [ContextMenu("Snap to Grid")]
    void Snap()
    {
        if (grid == null)
        {
            Debug.LogWarning("Grid reference is missing.");
            return;
        }

        Vector3Int cellPosition = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPosition);
    }
}
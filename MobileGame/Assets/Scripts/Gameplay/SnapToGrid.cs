using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SnapToGrid : MonoBehaviour
{
    public Grid grid;

    [ContextMenu("Snap to Grid")]
    private void Start()
    {
        // Try to find the Grid GameObject by name if not assigned
        if (grid == null)
        {
            GameObject gridObject = GameObject.Find("Grid");
            if (gridObject != null)
            {
                grid = gridObject.GetComponent<Grid>();
            }

            if (grid == null)
            {
                Debug.LogWarning("No Grid found named 'MainGrid'. Please assign one manually.");
                return;
            }
        }

        Snap();
    }

    void Snap()
    {
        Vector3Int cellPosition = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPosition);
    }
}
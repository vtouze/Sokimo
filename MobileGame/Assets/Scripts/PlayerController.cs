using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap topTilemap;

    [SerializeField] private float moveCooldown = 0.2f;

    private Vector3Int currentGridPos;
    private float lastMoveTime = 0f;

    private Vector3Int previousGridPos;

    void Start()
    {
        currentGridPos = groundTilemap.WorldToCell(transform.position);
        previousGridPos = currentGridPos;
    }

    void Update()
    {
        if (Time.time - lastMoveTime < moveCooldown)
            return;

        Vector3Int direction = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) direction = new Vector3Int(0, 1, 0);
        if (Input.GetKeyDown(KeyCode.S)) direction = new Vector3Int(0, -1, 0);
        if (Input.GetKeyDown(KeyCode.A)) direction = new Vector3Int(-1, 0, 0);
        if (Input.GetKeyDown(KeyCode.D)) direction = new Vector3Int(1, 0, 0);

        if (direction != Vector3Int.zero)
        {
            Vector3Int targetPos = currentGridPos + direction;

            if (groundTilemap.HasTile(targetPos) && !topTilemap.HasTile(targetPos))
            {
                previousGridPos = currentGridPos;  // Save previous position BEFORE moving
                currentGridPos = targetPos;
                transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);
                lastMoveTime = Time.time;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Coin coin = other.GetComponent<Coin>();
        if (coin != null)
        {
            coin.Collect();
        }
    }

    public Vector3Int GetPreviousGridPosition()
    {
        return previousGridPos;
    }

}
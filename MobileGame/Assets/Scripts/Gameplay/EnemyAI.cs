using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap topTilemap;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float moveCooldown = 0.2f;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite chaseSprite;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3Int currentGridPos;
    private bool isChasing = false;
    private float lastMoveTime = 0f;

    private readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0)
    };

    void Awake()
    {
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer not found on EnemyAI GameObject!");
    }

    void Start()
    {
        currentGridPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);

        // Assigner le sprite idle au départ
        if (spriteRenderer != null && idleSprite != null)
            spriteRenderer.sprite = idleSprite;
    }

    void Update()
    {
        if (Time.time - lastMoveTime < moveCooldown)
            return;

        Vector3Int playerGridPos = groundTilemap.WorldToCell(playerTransform.position);
        int manhattanDist = Mathf.Abs(playerGridPos.x - currentGridPos.x) + Mathf.Abs(playerGridPos.y - currentGridPos.y);

        if (!isChasing && manhattanDist == 1)
        {
            isChasing = true;

            // Changement du sprite au début de la chasse
            if (spriteRenderer != null && chaseSprite != null)
                spriteRenderer.sprite = chaseSprite;
        }

        if (!isChasing)
            return;

        if (manhattanDist > 1)
        {
            Vector3Int bestMove = currentGridPos;

            foreach (Vector3Int dir in directions)
            {
                Vector3Int candidate = currentGridPos + dir;
                int distToPlayer = Mathf.Abs(playerGridPos.x - candidate.x) + Mathf.Abs(playerGridPos.y - candidate.y);

                if (distToPlayer == 1 && IsWalkable(candidate))
                {
                    bestMove = candidate;
                    break;
                }
            }

            if (bestMove != currentGridPos)
            {
                currentGridPos = bestMove;
                transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);
                lastMoveTime = Time.time;
            }
        }
    }

    bool IsWalkable(Vector3Int pos)
    {
        return groundTilemap.HasTile(pos) && !topTilemap.HasTile(pos);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
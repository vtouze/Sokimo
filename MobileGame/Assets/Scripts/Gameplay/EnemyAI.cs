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

    public enum EnemyType { Sleeper, Patroller }
    [SerializeField] private EnemyType enemyType;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    void Start()
    {
        currentGridPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);

        if (spriteRenderer != null)
        {
            if (enemyType == EnemyType.Patroller && chaseSprite != null)
                spriteRenderer.sprite = chaseSprite;
            else if (enemyType == EnemyType.Sleeper && idleSprite != null)
                spriteRenderer.sprite = idleSprite;
        }
    }

    void Update()
    {
        if (enemyType == EnemyType.Sleeper)
        {
            Vector3Int playerPos = groundTilemap.WorldToCell(playerTransform.position);
            int dist = ManhattanDistance(currentGridPos, playerPos);
            TryChasePlayer(playerPos, dist);
        }
    }

    public void HandlePatrollerLogic(Vector3Int playerPos, int manhattanDist)
    {
        if (enemyType != EnemyType.Patroller) return;

        TryChasePlayer(playerPos, manhattanDist);

        if (!isChasing && patrolPoints.Length > 0)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            Vector3Int nextPos = groundTilemap.WorldToCell(patrolPoints[currentPatrolIndex].position);

            if (IsWalkable(nextPos))
            {
                currentGridPos = nextPos;
                transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);
            }
        }
    }

    private void TryChasePlayer(Vector3Int playerGridPos, int manhattanDist)
    {
        if (Time.time - lastMoveTime < moveCooldown)
            return;

        if (!isChasing && manhattanDist == 1)
        {
            isChasing = true;
            if (spriteRenderer != null && chaseSprite != null)
                spriteRenderer.sprite = chaseSprite;
        }

        if (!isChasing || manhattanDist <= 1)
            return;

        Vector3Int bestMove = currentGridPos;

        foreach (Vector3Int dir in directions)
        {
            Vector3Int candidate = currentGridPos + dir;
            int distToPlayer = ManhattanDistance(candidate, playerGridPos);

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

    private int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    bool IsWalkable(Vector3Int pos)
    {
        bool hasGround = groundTilemap.HasTile(pos);
        bool hasObstacle = topTilemap.HasTile(pos);

        return hasGround && !hasObstacle;
    }

    public void AnimateEnemyDeath(GameObject enemy)
    {
        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerController.BlockMovement(true);

        MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
            script.enabled = false;

        LeanTween.scale(enemy, Vector3.zero, 0.4f).setEase(LeanTweenType.easeInBack);
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            LeanTween.alpha(enemy, 0f, 0.4f).setEase(LeanTweenType.easeInQuad);
        }

        LeanTween.delayedCall(enemy, 0.45f, () => Destroy(enemy));
            if (playerController != null)
            playerController.BlockMovement(false);
    }
}
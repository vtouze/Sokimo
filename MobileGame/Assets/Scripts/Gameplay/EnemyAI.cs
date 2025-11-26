using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap topTilemap;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveCooldown = 0.5f;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite chaseSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3Int currentGridPos;
    private bool isChasing = false;
    private float lastMoveTime = 0f;
    private Coroutine patrolRoutine;

    private readonly Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0)
    };

    public enum EnemyType { Sleeper, Patroller, Sentinel }
    public EnemyType enemyType;

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

        // Check if enemy is already one grid away from player at start
        if (enemyType == EnemyType.Sleeper)
        {
            Vector3Int playerPos = groundTilemap.WorldToCell(playerTransform.position);
            int dist = ManhattanDistance(currentGridPos, playerPos);

            // If enemy is one grid away, start chasing immediately
            if (dist == 1)
            {
                isChasing = true;
                if (spriteRenderer != null && chaseSprite != null)
                    spriteRenderer.sprite = chaseSprite;
            }
        }

        // Start patrol routine for Patrollers
        if (enemyType == EnemyType.Patroller && patrolPoints.Length > 0)
        {
            patrolRoutine = StartCoroutine(PatrolRoutine());
        }
    }

    void OnDestroy()
    {
        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
        }
    }

    // This method is called from PlayerController after the player moves
    public void HandleEnemyLogic(Vector3Int playerPos)
    {
        if (enemyType == EnemyType.Sleeper)
        {
            int dist = ManhattanDistance(currentGridPos, playerPos);
            TryChasePlayer(playerPos, dist);
        }
        else if (enemyType == EnemyType.Sentinel)
        {
            HandleSentinelLogic(playerPos);
        }
        // Patrollers are handled by their own coroutine
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveCooldown);

            // Check if player is on the same grid
            Vector3Int playerPos = groundTilemap.WorldToCell(playerTransform.position);
            if (currentGridPos == playerPos)
            {
                PlayerController player = playerTransform.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.EnemyCollided(this);
                    yield break; // Stop patrolling if player is killed
                }
            }

            // Move to next patrol point
            if (patrolPoints.Length > 0)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                Vector3Int nextPos = groundTilemap.WorldToCell(patrolPoints[currentPatrolIndex].position);

                if (IsWalkable(nextPos))
                {
                    // Move to the next position
                    Vector3 targetWorldPos = groundTilemap.GetCellCenterWorld(nextPos);
                    transform.position = targetWorldPos;
                    currentGridPos = nextPos;

                    // Check again after moving in case player moved to this position
                    playerPos = groundTilemap.WorldToCell(playerTransform.position);
                    if (currentGridPos == playerPos)
                    {
                        PlayerController player = playerTransform.GetComponent<PlayerController>();
                        if (player != null)
                        {
                            player.EnemyCollided(this);
                            yield break;
                        }
                    }
                }
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

    public void HandleSentinelLogic(Vector3Int playerPos)
    {
        if (enemyType != EnemyType.Sentinel) return;
        if (Time.time - lastMoveTime < moveCooldown) return;

        if (IsInLineOfSight(playerPos))
        {
            isChasing = true;
            if (spriteRenderer != null && chaseSprite != null)
                spriteRenderer.sprite = chaseSprite;

            Vector3Int direction = Vector3Int.zero;
            if (playerPos.x > currentGridPos.x) direction = Vector3Int.right;
            else if (playerPos.x < currentGridPos.x) direction = Vector3Int.left;
            else if (playerPos.y > currentGridPos.y) direction = Vector3Int.up;
            else if (playerPos.y < currentGridPos.y) direction = Vector3Int.down;

            Vector3Int nextPos = currentGridPos + direction;
            if (IsWalkable(nextPos))
            {
                currentGridPos = nextPos;
                transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);
                lastMoveTime = Time.time;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(
                    groundTilemap.GetCellCenterWorld(currentGridPos), 0.1f
                );

                foreach (var col in colliders)
                {
                    EnemyAI otherEnemy = col.GetComponent<EnemyAI>();
                    if (otherEnemy != null && otherEnemy != this)
                    {
                        AnimateEnemyDeath(otherEnemy.gameObject);
                    }
                }

                if (currentGridPos == playerPos)
                {
                    PlayerController player = playerTransform.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        player.EnemyCollided(this);
                    }
                }

                if (spriteRenderer != null && (direction == Vector3Int.left || direction == Vector3Int.right))
                {
                    spriteRenderer.flipX = (direction == Vector3Int.right);
                }
            }
        }
        else
        {
            isChasing = false;
            if (spriteRenderer != null && idleSprite != null)
                spriteRenderer.sprite = idleSprite;
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

        PlayerController playerController = Object.FindFirstObjectByType<PlayerController>();
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

    private bool IsInLineOfSight(Vector3Int playerPos)
    {
        if (playerPos.x == currentGridPos.x)
        {
            int step = playerPos.y > currentGridPos.y ? 1 : -1;
            for (int y = currentGridPos.y + step; y != playerPos.y; y += step)
            {
                if (topTilemap.HasTile(new Vector3Int(playerPos.x, y, 0)))
                    return false;
            }
            return true;
        }
        else if (playerPos.y == currentGridPos.y)
        {
            int step = playerPos.x > currentGridPos.x ? 1 : -1;
            for (int x = currentGridPos.x + step; x != playerPos.x; x += step)
            {
                if (topTilemap.HasTile(new Vector3Int(x, playerPos.y, 0)))
                    return false;
            }
            return true;
        }
        return false;
    }
}
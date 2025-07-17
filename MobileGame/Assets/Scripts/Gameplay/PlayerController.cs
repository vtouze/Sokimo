using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap topTilemap;
    [SerializeField] private float moveCooldown = 0.2f;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomAmount = 3f;
    [SerializeField] private float zoomDuration = 0.5f;

    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private Vector3Int currentGridPos;
    private float lastMoveTime = 0f;

    private Vector2 touchStartPos;
    private bool swipeStarted = false;
    private float minSwipeDistance = 50f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private IdleFloat idleFloatScript;

    [SerializeField] private FadeManager fadeManager;

    private bool isDead = false;

    void Start()
    {
        currentGridPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);

        if (mainCamera == null)
            mainCamera = Camera.main;

        idleFloatScript = GetComponentInChildren<IdleFloat>();
        if (idleFloatScript == null)
            Debug.LogWarning("IdleFloat script not found on Player.");
    }

    void Update()
    {
        if (Time.time - lastMoveTime < moveCooldown)
            return;

        Vector3Int direction = GetInputDirection();

        if (direction != Vector3Int.zero)
        {
            TryMove(direction);
        }
    }

    private Vector3Int GetInputDirection()
    {
        Vector3Int dir = Vector3Int.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    swipeStarted = true;
                    break;

                case TouchPhase.Ended:
                    if (!swipeStarted) return Vector3Int.zero;

                    Vector2 swipe = touch.position - touchStartPos;
                    swipeStarted = false;

                    if (swipe.magnitude < minSwipeDistance) return Vector3Int.zero;

                    swipe.Normalize();

                    if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                        dir = swipe.x > 0 ? Vector3Int.right : Vector3Int.left;
                    else
                        dir = swipe.y > 0 ? Vector3Int.up : Vector3Int.down;
                    break;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W)) dir = Vector3Int.up;
            if (Input.GetKeyDown(KeyCode.S)) dir = Vector3Int.down;
            if (Input.GetKeyDown(KeyCode.A)) dir = Vector3Int.left;
            if (Input.GetKeyDown(KeyCode.D)) dir = Vector3Int.right;
        }

        return dir;
    }

    private void TryMove(Vector3Int direction)
    {
        if (isDead) return;

        Vector3Int targetPos = currentGridPos + direction;

        if (groundTilemap.HasTile(targetPos) && !topTilemap.HasTile(targetPos))
        {
            currentGridPos = targetPos;
            transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);
            lastMoveTime = Time.time;

            if (EnemyOnSameTile(currentGridPos))
            {
                StartCoroutine(DeathSequence());
            }
        }
    }

    public void TeleportTo(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        currentGridPos = groundTilemap.WorldToCell(worldPosition);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Coin coin))
        {
            coin.Collect();
        }
    }

    private bool EnemyOnSameTile(Vector3Int pos)
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (var enemy in enemies)
        {
            Vector3Int enemyGridPos = groundTilemap.WorldToCell(enemy.transform.position);
            if (enemyGridPos == pos)
                return true;
        }
        return false;
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;

        if (idleFloatScript != null)
            idleFloatScript.enabled = false;

        yield return StartCoroutine(ZoomAndFadeSprite());
        fadeManager.PlayFadeOutAndLoadScene(SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(fadeManager.fadeDuration);
        isDead = false;
    }


    private IEnumerator ZoomAndFadeSprite()
    {
        float startSize = mainCamera.orthographicSize;
        float targetSize = startSize / zoomAmount;

        float timer = 0f;
        while (timer < zoomDuration)
        {
            timer += Time.deltaTime;
            float t = timer / zoomDuration;
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
            yield return null;
        }

        timer = 0f;
        Color originalColor = spriteRenderer.color;
        while (timer < zoomDuration)
        {
            timer += Time.deltaTime;
            float t = timer / zoomDuration;
            float alpha = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    public void PlayPortalAnimation(Vector3Int targetGridPos, System.Action onComplete = null)
    {
        float duration = 0.5f;

        Vector3 originalScale = transform.localScale;
        Quaternion originalRotation = transform.rotation;

        LeanTween.scale(gameObject, originalScale * 0.5f, duration).setEaseInOutSine();
        LeanTween.rotateZ(gameObject, transform.eulerAngles.z + 180f, duration).setEaseInOutSine().setOnComplete(() =>
        {
            currentGridPos = targetGridPos;
            transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);

            LeanTween.scale(gameObject, originalScale, duration).setEaseInOutSine();
            LeanTween.rotate(gameObject, originalRotation.eulerAngles, duration).setEaseInOutSine().setOnComplete(() =>
            {
                onComplete?.Invoke();
            });
        });
    }
}
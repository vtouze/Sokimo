using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Advertisements;

public class PlayerController : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap topTilemap;
    [SerializeField] private float moveCooldown = 0.1f;

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
    [SerializeField] private IdleFloat idleFloatScript;

    [SerializeField] private FadeManager fadeManager;

    [HideInInspector] public bool isBlocked = false;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip killSound;
    public AudioClip grabSound;
    public AudioClip unlockSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip teleportSound;
    public AudioSource audioSource;

    void Start()
    {
        currentGridPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        if (isBlocked) return;

        Vector3Int targetPos = currentGridPos + direction;
        Vector3 targetWorldPos = groundTilemap.GetCellCenterWorld(targetPos);

        if (!groundTilemap.HasTile(targetPos))
            return;

        if (topTilemap.HasTile(targetPos))
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetWorldPos, 0.1f);
        Door door = null;
        foreach (var col in colliders)
        {
            door = col.GetComponent<Door>();
            if (door != null)
                break;
        }

        var itemSystem = GetComponent<PlayerItemSystem>();

        if (door != null)
        {
            if (door.IsLocked)
            {
                if (itemSystem != null && itemSystem.HasKey)
                {
                    door.OpenDoor();
                    itemSystem.ConsumeKey();

                    currentGridPos = targetPos;
                    transform.position = targetWorldPos;
                    lastMoveTime = Time.time;
                }
            }
            else
            {
                currentGridPos = targetPos;
                transform.position = targetWorldPos;
                lastMoveTime = Time.time;
            }
        }
        else
        {
            currentGridPos = targetPos;
            transform.position = targetWorldPos;
            lastMoveTime = Time.time;
            PlaySound(moveSound);
            if (EnemyOnSameTile(currentGridPos))
            {
                if (itemSystem != null && itemSystem.HasSword)
                {
                    KillEnemiesOnSameTile(currentGridPos);
                    itemSystem.ConsumeSword();
                }
                else
                {
                    CoinManager.Instance?.ClearSessionCoins();
                    PlaySound(deathSound);
                    StartCoroutine(EndingSequence(SceneManager.GetActiveScene().name));
                }
            }
        }

        foreach (var col in colliders)
        {
            if (col.CompareTag("RedFlag"))
            {
                PlaySound(winSound);
                CoinManager.Instance?.CommitSessionCoins();
                StartCoroutine(EndingSequence("MainMenu"));
                return;
            }
        }

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (var enemy in enemies)
        {
            Vector3Int playerPos = currentGridPos;
            Vector3Int enemyPos = groundTilemap.WorldToCell(enemy.transform.position);
            int dist = Mathf.Abs(playerPos.x - enemyPos.x) + Mathf.Abs(playerPos.y - enemyPos.y);
            enemy.HandlePatrollerLogic(playerPos, dist);
            enemy.HandleSentinelLogic(playerPos);
        }
    }


    private void KillEnemiesOnSameTile(Vector3Int pos)
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (var enemy in enemies)
        {
            Vector3Int enemyGridPos = groundTilemap.WorldToCell(enemy.transform.position);
            if (enemyGridPos == pos)
            {
                enemy.AnimateEnemyDeath(enemy.gameObject);
                PlaySound(killSound);
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
            PlaySound(coinSound);
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

    public IEnumerator EndingSequence(string sceneName)
    {
        BlockMovement(true);

        if (idleFloatScript != null)
            idleFloatScript.enabled = false;

        int originalSortingOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = 100;

        yield return StartCoroutine(ZoomAndFadeSprite());

        fadeManager.PlayRawFadeOut();
        yield return new WaitForSeconds(fadeManager.fadeDuration);

        SceneManager.LoadScene(sceneName);
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
        BlockMovement(true);
        float duration = 0.5f;

        Vector3 originalScale = transform.localScale;
        Quaternion originalRotation = transform.rotation;

        PlaySound(teleportSound);
        LeanTween.scale(gameObject, originalScale * 0.5f, duration).setEaseInOutSine();
        LeanTween.rotateZ(gameObject, transform.eulerAngles.z + 180f, duration).setEaseInOutSine().setOnComplete(() =>
        {
            currentGridPos = targetGridPos;
            transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);

            LeanTween.scale(gameObject, originalScale, duration).setEaseInOutSine();
            LeanTween.rotate(gameObject, originalRotation.eulerAngles, duration).setEaseInOutSine().setOnComplete(() =>
            {
                onComplete?.Invoke();
                BlockMovement(false);
            });
        });
    }

    public void BlockMovement(bool value)
    {
        isBlocked = value;
    }

    public void EnemyCollided(EnemyAI enemy)
    {
        var itemSystem = GetComponent<PlayerItemSystem>();
        if (itemSystem != null && itemSystem.HasSword)
        {
            enemy.AnimateEnemyDeath(enemy.gameObject);
            itemSystem.ConsumeSword();
        }
        else
        {
            PlaySound(deathSound);
            CoinManager.Instance?.ClearSessionCoins();
            StartCoroutine(EndingSequence(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            ));
        }
    }

    public void PlaySound(AudioClip clip)
    {
        bool masterMuted = PlayerPrefs.GetInt("MasterMuted", 0) == 1;
        bool sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        if (clip != null && audioSource != null && sfxEnabled && !masterMuted)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
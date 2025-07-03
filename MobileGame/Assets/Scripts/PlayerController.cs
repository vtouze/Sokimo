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

    // For swipe detection
    private Vector2 touchStartPos;
    private bool swipeStarted = false;
    private float minSwipeDistance = 50f; // Minimum pixels for a valid swipe

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

        // Touch input for swipe detection
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
                    if (!swipeStarted) break;

                    Vector2 touchEndPos = touch.position;
                    Vector2 swipeVector = touchEndPos - touchStartPos;

                    if (swipeVector.magnitude >= minSwipeDistance)
                    {
                        swipeVector.Normalize();

                        // Determine swipe direction (up, down, left, right)
                        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
                        {
                            // Horizontal swipe
                            if (swipeVector.x > 0)
                                direction = new Vector3Int(1, 0, 0);   // Right
                            else
                                direction = new Vector3Int(-1, 0, 0);  // Left
                        }
                        else
                        {
                            // Vertical swipe
                            if (swipeVector.y > 0)
                                direction = new Vector3Int(0, 1, 0);   // Up
                            else
                                direction = new Vector3Int(0, -1, 0);  // Down
                        }
                    }
                    swipeStarted = false;
                    break;
            }
        }
        else
        {
            // Optional: handle keyboard input here as fallback
            if (Input.GetKeyDown(KeyCode.W)) direction = new Vector3Int(0, 1, 0);
            if (Input.GetKeyDown(KeyCode.S)) direction = new Vector3Int(0, -1, 0);
            if (Input.GetKeyDown(KeyCode.A)) direction = new Vector3Int(-1, 0, 0);
            if (Input.GetKeyDown(KeyCode.D)) direction = new Vector3Int(1, 0, 0);
        }

        if (direction != Vector3Int.zero)
        {
            Move(direction);
        }
    }

    public void Move(Vector3Int direction)
    {
        if (Time.time - lastMoveTime < moveCooldown)
            return;

        Vector3Int targetPos = currentGridPos + direction;

        if (groundTilemap.HasTile(targetPos) && !topTilemap.HasTile(targetPos))
        {
            previousGridPos = currentGridPos;
            currentGridPos = targetPos;
            transform.position = groundTilemap.GetCellCenterWorld(currentGridPos);
            lastMoveTime = Time.time;
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

    public Vector3Int GetCurrentGridPosition()
    {
        return currentGridPos;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class CoinFlyer : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.1f;

    private RectTransform flyerRect;
    private RectTransform uiCoinIcon;
    private Canvas canvas;

    void Awake()
    {
        flyerRect = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();

        GameObject iconGO = GameObject.Find("Coin_Image");
        if (iconGO != null)
        {
            uiCoinIcon = iconGO.GetComponent<RectTransform>();
        }
    }

    public void LaunchToUI()
    {
        if (canvas == null || uiCoinIcon == null) return;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        transform.SetParent(canvas.transform, false);
        flyerRect.anchoredPosition = localPoint;

        Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiCoinIcon.position);
        Vector2 targetLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            targetScreenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out targetLocalPoint
        );

        LeanTween.move(flyerRect, targetLocalPoint, moveDuration).setEaseInOutQuad().setOnComplete(() =>
        {
            LeanTween.scale(uiCoinIcon, Vector3.one * pulseScale, pulseDuration).setLoopPingPong(1);
            CoinManager.Instance?.AddCoin();
            Destroy(gameObject);
        });
    }
}
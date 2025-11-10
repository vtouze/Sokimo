using UnityEngine;
using UnityEngine.UI;

public class CoinFlyer : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.1f;

    [SerializeField] private string coinIconName = "Coin_Image";
    [SerializeField] private string coinTextName = "Coin_Text";

    private RectTransform flyerRect;
    private RectTransform uiCoinIcon;
    private RectTransform uiTextToPulse;
    private Canvas canvas;

    void Awake()
    {
        flyerRect = GetComponent<RectTransform>();
        canvas = FindAnyObjectByType<Canvas>();

        GameObject iconGO = GameObject.Find(coinIconName);
        if (iconGO != null)
        {
            uiCoinIcon = iconGO.GetComponent<RectTransform>();
        }

        GameObject textGO = GameObject.Find(coinTextName);
        if (textGO != null)
        {
            uiTextToPulse = textGO.GetComponent<RectTransform>();
        }
    }

    public void LaunchToUIFromWorld()
    {
        if (canvas == null || uiCoinIcon == null) return;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localStart
        );

        transform.SetParent(canvas.transform, false);
        flyerRect.anchoredPosition = localStart;

        MoveToTarget();
    }

    public void LaunchToUIFromUI(RectTransform fromUI)
    {
        if (canvas == null || uiCoinIcon == null) return;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, fromUI.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localStart
        );

        transform.SetParent(canvas.transform, false);
        flyerRect.anchoredPosition = localStart;

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiCoinIcon.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            targetScreenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 targetLocalPoint
        );

        LeanTween.move(flyerRect, targetLocalPoint, moveDuration).setEaseInOutQuad().setOnComplete(() =>
        {
            LeanTween.scale(uiCoinIcon, Vector3.one * pulseScale, pulseDuration).setLoopPingPong(1);

            if (uiTextToPulse != null)
            {
                LeanTween.scale(uiTextToPulse, Vector3.one * pulseScale, pulseDuration).setLoopPingPong(1);
            }

            CoinManager.Instance?.AddSessionCoin();
            Destroy(gameObject);
        });
    }
}
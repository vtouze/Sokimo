using UnityEngine;
using UnityEngine.UI;

public class PurchaseCompleteAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform coinBalanceIcon;
    [SerializeField] private RectTransform coin20Icon;
    [SerializeField] private RectTransform coin50Icon;
    [SerializeField] private RectTransform coin100Icon;
    [SerializeField] private RectTransform dailyRewardIcon;
    [SerializeField] private RectTransform adRewardIcon;
    [SerializeField] private Button noAdsButton;

    [Header("Settings")]
    [SerializeField] private float coinMoveDuration = 0.8f;
    [SerializeField] private float coinBounceScale = 1.3f;
    [SerializeField] private float coinBounceDuration = 0.25f;

    [SerializeField] private float noAdsScaleUp = 1.15f;
    [SerializeField] private float noAdsScaleDuration = 0.3f;
    [SerializeField] private Color noAdsLockedColor = Color.gray;

    public static PurchaseCompleteAnimations Instance;

    void Awake()
    {
        Instance = this;
    }

    // --------------------------
    // GENERIC COIN ANIMATION
    // --------------------------
    public void PlayCoinAnimation(string coinType)
    {
        RectTransform sourceIcon = null;

        switch (coinType)
        {
            case "coin20": sourceIcon = coin20Icon; break;
            case "coin50": sourceIcon = coin50Icon; break;
            case "coin100": sourceIcon = coin100Icon; break;
        }

        if (sourceIcon == null)
        {
            Debug.LogWarning($"[PurchaseAnimations] Unknown or unassigned coin icon for '{coinType}'");
            return;
        }

        PlayCoinAnimationFrom(sourceIcon);
    }

    // 🟢 Reusable function — works for daily reward & ad rewarded
    public void PlayCoinAnimationFrom(RectTransform sourceIcon)
    {
        if (coinBalanceIcon == null || sourceIcon == null)
        {
            Debug.LogWarning("[PurchaseAnimations] Missing coin references for coin animation!");
            return;
        }

        RectTransform tempIcon = Instantiate(sourceIcon, sourceIcon.parent);
        tempIcon.SetAsLastSibling();

        Vector3 startPos = sourceIcon.position;
        Vector3 targetPos = coinBalanceIcon.position;

        tempIcon.position = startPos;
        tempIcon.localScale = sourceIcon.localScale;

        // Move animation (unchanged)
        LeanTween.move(tempIcon.gameObject, targetPos, coinMoveDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Destroy(tempIcon.gameObject);

                Vector3 originalScale = coinBalanceIcon.localScale;
                Vector3 targetScale = originalScale * coinBounceScale;

                // 🌀 Floaty bounce: slower ease in/out, like air resistance
                LeanTween.scale(coinBalanceIcon, targetScale, coinBounceDuration / 1.2f)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(coinBalanceIcon, originalScale, coinBounceDuration / 1.5f)
                            .setEase(LeanTweenType.easeOutElastic); // softer landing curve
                    });
            });
    }

    // --------------------------
    // NO ADS ANIMATION
    // --------------------------
    public void PlayNoAdsAnimation()
    {
        if (noAdsButton == null)
        {
            Debug.LogWarning("[PurchaseAnimations] Missing No Ads button reference!");
            return;
        }

        RectTransform btnRect = noAdsButton.GetComponent<RectTransform>();
        Image btnImage = noAdsButton.GetComponent<Image>();
        Vector3 originalScale = btnRect.localScale;
        Vector3 targetScale = originalScale * noAdsScaleUp;

        LeanTween.scale(btnRect, targetScale, noAdsScaleDuration)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                LeanTween.scale(btnRect, originalScale, noAdsScaleDuration)
                    .setEase(LeanTweenType.easeInOutBack)
                    .setOnComplete(() =>
                    {
                        if (btnImage != null)
                            btnImage.color = noAdsLockedColor;

                        noAdsButton.interactable = false;
                    });
            });
    }

    // --------------------------
    // CONVENIENCE HELPERS
    // --------------------------
    public void PlayDailyRewardAnimation() => PlayCoinAnimationFrom(dailyRewardIcon);
    public void PlayAdRewardAnimation() => PlayCoinAnimationFrom(adRewardIcon);
}
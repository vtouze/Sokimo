using UnityEngine;
using UnityEngine.UI;

public class PurchaseCompleteAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform coinBalanceIcon;
    [SerializeField] private RectTransform coin20Icon;
    [SerializeField] private RectTransform coin50Icon;
    [SerializeField] private RectTransform coin100Icon;
    [SerializeField] private RectTransform coin150Icon;
    [SerializeField] private RectTransform coin200Icon;
    [SerializeField] private RectTransform dailyRewardIcon;

    [Header("Settings")]
    [SerializeField] private float coinMoveDuration = 0.8f;
    [SerializeField] private float coinBounceScale = 1.3f;
    [SerializeField] private float coinBounceDuration = 0.25f;

    public static PurchaseCompleteAnimations Instance;

    void Awake()
    {
        Instance = this;
    }
    public void PlayCoinAnimation(string coinType)
    {
        RectTransform sourceIcon = null;

        switch (coinType)
        {
            case "coin20": sourceIcon = coin20Icon; break;
            case "coin50": sourceIcon = coin50Icon; break;
            case "coin100": sourceIcon = coin100Icon; break;
            case "coin150": sourceIcon = coin150Icon; break;
            case "coin200": sourceIcon = coin200Icon; break;
        }

        PlayCoinAnimationFrom(sourceIcon);
    }

    public void PlayCoinAnimationFrom(RectTransform sourceIcon)
    {
        RectTransform tempIcon = Instantiate(sourceIcon, sourceIcon.parent);
        tempIcon.SetAsLastSibling();

        Vector3 startPos = sourceIcon.position;
        Vector3 targetPos = coinBalanceIcon.position;

        tempIcon.position = startPos;
        tempIcon.localScale = sourceIcon.localScale;

        LeanTween.move(tempIcon.gameObject, targetPos, coinMoveDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Destroy(tempIcon.gameObject);

                Vector3 originalScale = coinBalanceIcon.localScale;
                Vector3 targetScale = originalScale * coinBounceScale;

                LeanTween.scale(coinBalanceIcon, targetScale, coinBounceDuration / 1.2f)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(coinBalanceIcon, originalScale, coinBounceDuration / 1.5f)
                            .setEase(LeanTweenType.easeOutElastic);
                    });
            });
    }
    public void PlayDailyRewardAnimation() => PlayCoinAnimationFrom(dailyRewardIcon);
}
/*using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    private const string NoAdsKey = "NoAdsPurchased";
    private bool noAdsEnabled = false;

    [Header("References (Optional)")]
    [SerializeField] private AdsBanner adsBanner;
    [SerializeField] private AdsInterstitial adsInterstitial;

    [Header("Debug Options")]
    [Tooltip("Check this in the Inspector to reset the No Ads key for testing.")]
    [SerializeField] private bool resetNoAdsKey = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CheckNoAdsStatus();
    }

    void Update()
    {
        // 👇 Debug toggle in Inspector
        if (resetNoAdsKey)
        {
            ResetNoAdsKey();
            resetNoAdsKey = false; // Auto-uncheck after use
        }
    }

    public void CheckNoAdsStatus()
    {
        noAdsEnabled = PlayerPrefs.GetInt(NoAdsKey, 0) == 1;
        Debug.Log($"[AdsManager] No Ads status: {(noAdsEnabled ? "ENABLED" : "DISABLED")}");

        if (noAdsEnabled)
            DisableAllAds();
    }

    public void DisableAllAds()
    {
        Debug.Log("[AdsManager] Disabling all ads...");

        if (adsInterstitial != null)
            adsInterstitial.enabled = false;

        if (adsBanner != null)
        {
            adsBanner.StopAllCoroutines();
            UnityEngine.Advertisements.Advertisement.Banner.Hide();
            adsBanner.enabled = false;
        }

        Debug.Log("[AdsManager] All ads disabled successfully.");
    }

    public void OnNoAdsPurchased()
    {
        PlayerPrefs.SetInt(NoAdsKey, 1);
        PlayerPrefs.Save();

        noAdsEnabled = true;
        DisableAllAds();

        Debug.Log("[AdsManager] ✅ No Ads purchase confirmed and saved.");
    }

    // 🔧 Debug/Test helper — called when resetNoAdsKey is true
    private void ResetNoAdsKey()
    {
        PlayerPrefs.DeleteKey(NoAdsKey);
        PlayerPrefs.Save();
        noAdsEnabled = false;

        Debug.Log("[AdsManager] 🔄 No Ads key reset. Ads will show again after reloading.");

        if (adsBanner != null) adsBanner.enabled = true;
        if (adsInterstitial != null) adsInterstitial.enabled = true;
    }

    public bool IsNoAdsEnabled() => noAdsEnabled;
}*/
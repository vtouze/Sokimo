using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private string coinID;
    public string CoinID => coinID;
    [SerializeField] private GameObject coinFlyerPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool alreadyCollected = false;

    void Start()
    {
        if (PlayerPrefs.GetInt("CoinCollected_" + coinID, 0) == 1)
        {
            alreadyCollected = true;
            DisableVisual();
        }
    }

    public void Collect()
    {
        if (alreadyCollected) return;

        alreadyCollected = true;

        PlayerPrefs.SetInt("CoinCollected_" + coinID, 1);
        PlayerPrefs.Save();

        //DeviceShakeManager.Instance?.Shake(ShakeType.Light);
        SpawnCoinFlyer();
        Destroy(gameObject);
    }

    private void SpawnCoinFlyer()
    {
        if (coinFlyerPrefab == null) return;

        GameObject flyer = Instantiate(coinFlyerPrefab, transform.position, Quaternion.identity);
        CoinFlyer coinFlyer = flyer.GetComponent<CoinFlyer>();

        if (coinFlyer != null)
        {
            coinFlyer.LaunchToUIFromWorld();
        }
    }

    private void DisableVisual()
    {
        if (spriteRenderer != null)
        {
            Color fadedColor = spriteRenderer.color;
            fadedColor.a = 0.3f;
            spriteRenderer.color = fadedColor;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }
}
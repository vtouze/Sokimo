using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private GameObject coinFlyerPrefab;

    public void Collect()
    {
        // Optional: play sound, disable visuals, etc.
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
}
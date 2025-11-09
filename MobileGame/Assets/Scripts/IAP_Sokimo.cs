using UnityEngine;
using UnityEngine.Purchasing;

public class IAP_Sokimo : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    public void Purchase(Product product)
    {
        PlaySound(buttonClickSound);
        string id = product.definition.id;

        if (id.Equals("coin20"))
        {
            CoinManager.Instance.AddCoin(20);
            PurchaseCompleteAnimations.Instance?.PlayCoinAnimation("coin20");
        }
        else if (id.Equals("coin50"))
        {
            CoinManager.Instance.AddCoin(50);
            PurchaseCompleteAnimations.Instance?.PlayCoinAnimation("coin50");
        }
        else if (id.Equals("coin100"))
        {
            CoinManager.Instance.AddCoin(100);
            PurchaseCompleteAnimations.Instance?.PlayCoinAnimation("coin100");
        }
        else if (id.Equals("coin150"))
        {
            CoinManager.Instance.AddCoin(150);
            PurchaseCompleteAnimations.Instance?.PlayCoinAnimation("coin150");
        }
        else if (id.Equals("coin200"))
        {
            CoinManager.Instance.AddCoin(200);
            PurchaseCompleteAnimations.Instance?.PlayCoinAnimation("coin200");
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
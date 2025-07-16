using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitcher : MonoBehaviour
{
    [SerializeField] private Image flagImage;
    [SerializeField] private Sprite[] flagSprites;

    private int currentLanguageIndex = 0;

    public void OnLanguageButtonPressed()
    {
        // Cycle to next language
        currentLanguageIndex = (currentLanguageIndex + 1) % flagSprites.Length;

        // Swap the flag sprite
        flagImage.sprite = flagSprites[currentLanguageIndex];

        /* 
        // Language logic placeholder:
        string[] languages = { "en", "fr", "es" }; // Add more if needed
        string selectedLanguage = languages[currentLanguageIndex];

        // Apply the selected language (you'll implement this later)
        LocalizationManager.SetLanguage(selectedLanguage); 
        PlayerPrefs.SetString("language", selectedLanguage);
        PlayerPrefs.Save();
        */
    }
}
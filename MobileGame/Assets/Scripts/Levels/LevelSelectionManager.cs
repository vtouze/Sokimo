using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class LevelSelectionManager : MonoBehaviour
{
    public static int _currentLevel = 0;

    [Header("Level Buttons")]
    public List<LevelData> levels;

    private void Start()
    {
        UpdateLevelButtons();
    }

    void UpdateLevelButtons()
    {
        int pieces = CoinManager.Instance?.CurrentCoins ?? 0;

        for (int i = 0; i < levels.Count; i++)
        {
            LevelData level = levels[i];

            bool isUnlocked = pieces >= level.requiredPieces;
            Button btn = level.buttonObject.GetComponent<Button>();
            btn.interactable = isUnlocked;

            if (isUnlocked)
            {
                level.lockImage.gameObject.SetActive(true);
                level.Level_Text.gameObject.SetActive(true);
                level.piecesContainer.SetActive(false);
            }
            else
            {
                level.lockImage.sprite = level.lockSprite;
                level.lockImage.gameObject.SetActive(true);
                level.Level_Text.gameObject.SetActive(false);
                level.piecesContainer.SetActive(true);

                TMP_Text piecesText = level.piecesContainer.GetComponentInChildren<TMP_Text>();
                if (piecesText != null)
                    piecesText.text = $"{level.requiredPieces}";
            }
        }
    }

    public void OnClickLevel(int levelIndex)
    {
        _currentLevel = levelIndex;
        SceneManager.LoadScene("LevelSelectionScene");
    }
}
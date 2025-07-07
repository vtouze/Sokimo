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
        int pieces = GameData.Pieces;
        Debug.Log($"You have {pieces} pieces. Total levels: {levels.Count}");

        for (int i = 0; i < levels.Count; i++)
        {
            LevelData level = levels[i];
            Debug.Log($"Setting up Level {i + 1}, requires {level.requiredPieces} pieces — button: {level.buttonObject.name}");

            bool isUnlocked = pieces >= level.requiredPieces;
            Button btn = level.buttonObject.GetComponent<Button>();
            btn.interactable = isUnlocked;

            if (isUnlocked)
            {
                level.lockImage.gameObject.SetActive(true);
                level.Level_Text.gameObject.SetActive(true);
                level.piecesContainer.SetActive(false); // ✅ Hide container when unlocked
            }
            else
            {
                level.lockImage.sprite = level.lockSprite;
                level.lockImage.gameObject.SetActive(true);
                level.Level_Text.gameObject.SetActive(false);
                level.piecesContainer.SetActive(true);  // ✅ Show container when locked

                // ✅ Optional: Update text inside the container if needed
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
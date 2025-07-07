using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class LevelData
{
    public GameObject buttonObject;      // The button itself
    public int requiredPieces;           // Pieces required to unlock

    public Image lockImage;              // Image component to show the lock icon
    public Sprite lockSprite;            // The lock sprite to display when locked

    public TMP_Text Level_Text;          // Text component displaying level number
    public GameObject piecesContainer;   // Contains both the icon and Pieces_Text
}
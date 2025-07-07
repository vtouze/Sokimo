using UnityEngine;

public static class GameData
{
    public static int Pieces
    {
        get => PlayerPrefs.GetInt("Pieces", 0);
        set => PlayerPrefs.SetInt("Pieces", value); //GameData.Pieces += 1;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] _levels;

    private void Start()
    {

        _levels[LevelSelectionManager._currentLevel].SetActive(true);
    }

}
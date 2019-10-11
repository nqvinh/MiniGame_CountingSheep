using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BayatGames.SaveGameFree;

public class CustomMenu : Editor
{
    [MenuItem("Tools/Data/ClearData")]
    private static void GenerateCode()
    {
        SaveGame.Clear();
        PlayerPrefs.DeleteAll();
    }
}


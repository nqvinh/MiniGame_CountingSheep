using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLocalAccess : DataAccessInterface
{
    PlayerData currentPlayerData = null;

    public void LoadData()
    {
        currentPlayerData = SaveGameServices.Instance.Load();
    }

    public PlayerData GetPlayerData()
    {
        if (currentPlayerData == null)
            currentPlayerData = SaveGameServices.Instance.Load();
        return currentPlayerData;
    }

    public void SavePlayerData(PlayerData data)
    {
        currentPlayerData = data;
        SaveGameServices.Instance.Save(data);
    }
}

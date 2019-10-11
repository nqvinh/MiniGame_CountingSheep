using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    DataAccessInterface dataAccess;

    public void LoadData(System.Action<bool> loadDone)
    {
        //Using Local Interface
        dataAccess = new DataLocalAccess();
        dataAccess.LoadData();
        if (dataAccess.GetPlayerData().userPlayTime == 0)
        {
            CreateNewData();
        }
        if (loadDone != null)
            loadDone(true);
    }

    public PlayerData PlayerData
    {
        get
        {
            return dataAccess.GetPlayerData();
        }
    }

    public void SavePlayerData()
    {
        PlayerData playerData = dataAccess.GetPlayerData();
        playerData.sheepDatas = GameController.Instance.GetSavingSheepData();
        dataAccess.SavePlayerData(playerData);
    }

    void CreateNewData()
    {
        for (int i = 0; i < GameController.numOfSheepType; ++i)
        {
           DataManager.Instance.PlayerData.sheepPriceCurrent.Add(ConfigManager.Instance.GetSheepConfigByType(i+1).Price);
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }
}

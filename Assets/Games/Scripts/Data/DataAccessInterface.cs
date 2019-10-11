using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DataAccessInterface 
{

    void LoadData();

    PlayerData GetPlayerData();

    void SavePlayerData(PlayerData data);
}

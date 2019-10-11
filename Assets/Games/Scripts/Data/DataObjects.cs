using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataObjects 
{
  
}

[Serializable]
public class SheepData
{
    public int sheepType;
    public int sheepState;
    public Vector3 position;
    public Vector3 localEulerAngles;
}

public class PlayerData
{
    public int userPlayTime = 0;
    public string userID;
    public double userSheepCoin;
    public int userLevel;
    public long userExp;
    public List<SheepData> sheepDatas;
    public List<double> sheepPriceCurrent;

    public float speedUpTimePoint = -1;
    public float x2SheepValueTimePoint = -1;
    public float autoMergeTimePoint = -1;
    public float superBoxTimePoint = -1;

    public PlayerData()
    {
        userID = System.DateTime.Now.ToString();
        userSheepCoin = 0;
        userLevel = 1;
        userExp = 0;
        userPlayTime = 0;
        sheepDatas = new List<SheepData>();
        sheepPriceCurrent = new List<double>();
        speedUpTimePoint = -1;
        x2SheepValueTimePoint = -1;
        autoMergeTimePoint = -1;
        superBoxTimePoint = -1;
    }
}

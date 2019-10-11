using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BoostType
{
    SheepSpeedUp,
    x2SheepValue,
    AutoMerge,
    SuperBox,
}
public class BoostTimer : Singleton<BoostTimer>
{
    readonly float SheepSpeedUpTime = 5*60;
    readonly float x2SheepValueTime = 3*60;
    readonly float autoMergeTime = 3*60;
    readonly float superBoxTime = 4*60;

    float speedUpTimePoint = -1;
    float x2SheepValueTimePoint = -1;
    float autoMergeTimePoint = -1;
    float superBoxTimePoint = -1;

    bool isBoostSpeedUp = false;
    bool isX2SheepValue = false;
    bool isAutoMerge = false;
    bool isSuperBoxTime = false;

    public bool IsBoostSpeedUp { get => isBoostSpeedUp; private set => isBoostSpeedUp = value; }
    public bool IsX2SheepValue { get => isX2SheepValue; private set => isX2SheepValue = value; }
    public bool IsAutoMerge { get => isAutoMerge;private set => isAutoMerge = value; }
    public bool IsSuperBoxTime { get => isSuperBoxTime;private set => isSuperBoxTime = value; }

    public void SetUp()
    {
        speedUpTimePoint = DataManager.Instance.PlayerData.speedUpTimePoint;
        x2SheepValueTimePoint = DataManager.Instance.PlayerData.x2SheepValueTimePoint;
        autoMergeTimePoint = DataManager.Instance.PlayerData.autoMergeTimePoint;
        superBoxTimePoint = DataManager.Instance.PlayerData.superBoxTimePoint;
        isBoostSpeedUp = speedUpTimePoint > 0;
        isX2SheepValue = x2SheepValueTimePoint >0;
        isAutoMerge = autoMergeTimePoint >0;
        isSuperBoxTime = superBoxTimePoint >0;
    }
    private void Update()
    {
        if (IsBoostSpeedUp)
        {
            speedUpTimePoint -= Time.deltaTime;
            if (speedUpTimePoint <=0)
            {
                speedUpTimePoint = -1;
                IsBoostSpeedUp = false;
            }
            DataManager.Instance.PlayerData.speedUpTimePoint = speedUpTimePoint;
        }

        if (IsX2SheepValue)
        {
            x2SheepValueTimePoint -= Time.deltaTime;
            if (x2SheepValueTimePoint > x2SheepValueTime)
            {
                x2SheepValueTimePoint = -1;
                IsX2SheepValue = false;
            }
            DataManager.Instance.PlayerData.x2SheepValueTimePoint = x2SheepValueTimePoint;
        }

        if (IsAutoMerge)
        {
            autoMergeTimePoint -= Time.deltaTime;
            if (autoMergeTimePoint > autoMergeTime)
            {
                autoMergeTimePoint = -1;
                IsAutoMerge = false;
            }
            DataManager.Instance.PlayerData.autoMergeTimePoint = autoMergeTimePoint;
        }

        if (IsSuperBoxTime)
        {
            superBoxTimePoint -= Time.deltaTime;
            if (superBoxTimePoint > superBoxTime)
            {
                speedUpTimePoint = -1;
                IsSuperBoxTime = false;
            }
            DataManager.Instance.PlayerData.superBoxTimePoint = superBoxTimePoint;
        }
    }

    public void ActiveBoost(BoostType type)
    {
        switch (type)
        {
            case BoostType.SheepSpeedUp:
                speedUpTimePoint += SheepSpeedUpTime;
                DataManager.Instance.PlayerData.speedUpTimePoint = speedUpTimePoint;
                IsBoostSpeedUp = true;
                break;
            case BoostType.x2SheepValue:
                x2SheepValueTimePoint += x2SheepValueTime;
                DataManager.Instance.PlayerData.x2SheepValueTimePoint = x2SheepValueTimePoint;
                IsX2SheepValue = true;
                break;
            case BoostType.AutoMerge:
                autoMergeTimePoint += autoMergeTime;
                DataManager.Instance.PlayerData.autoMergeTimePoint = autoMergeTimePoint;
                IsAutoMerge = true;
                break;
            case BoostType.SuperBox:
                superBoxTimePoint += superBoxTime;
                DataManager.Instance.PlayerData.superBoxTimePoint = superBoxTimePoint;
                IsSuperBoxTime = true;
                break;
            default:
                break;
        }
    }
}

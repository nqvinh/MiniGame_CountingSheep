using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    SheepConfig sheepConfig;
    LevelConfig levelConfig;


    public SheepConfig SheepConfigProp { get => sheepConfig; set => sheepConfig = value; }
    public LevelConfig LevelConfigProp { get => levelConfig; set => levelConfig = value; }

    public void LoadConfig()
    {
        sheepConfig = Resources.Load<SheepConfig>("Configs/SheepConfig");
        levelConfig = Resources.Load<LevelConfig>("Configs/LevelConfig");
    }

    public SheepConfigData GetSheepConfigByType(int type)
    {
        return Array.Find(sheepConfig.dataArray, sheep => sheep.Sheeptype == type);
    }

    public LevelConfigData GetLevelConfigByLevel(int lv)
    {
        return Array.Find(levelConfig.dataArray, lvl => lvl.Level == lv);
    }

    public int GetSheepMaxAtLevel(int lv)
    {
        int type = 1;
        SheepConfigData configData = Array.Find(sheepConfig.dataArray, sheep => sheep.Unlocklevel == lv);
        if (configData != null)
            type = configData.Sheeptype;
        return type;
    }
}

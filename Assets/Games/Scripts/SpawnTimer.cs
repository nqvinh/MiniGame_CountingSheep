using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTimer : Singleton<SpawnTimer>
{
    [SerializeField]
    float minTimeToSpawn=10;
    [SerializeField]
    float maxTimeToSpawn=15;

    float lastSpawnTime;
    float nextSpawnTime;

    bool hasSetup = false;

    public void Setup()
    {
        float timeOffset = Random.Range(minTimeToSpawn, maxTimeToSpawn);
        float isSuperBox = BoostTimer.Instance.IsSuperBoxTime ? 0.5f : 1f;
        timeOffset *= isSuperBox;
        nextSpawnTime = Time.time + timeOffset;
        hasSetup = true;
    }
    private void Update()
    {
        if (hasSetup)
        {
            if (Time.time > nextSpawnTime)
            {
                Setup();
                SpawnRandomSheep();
            }
        }
    }

    private void SpawnRandomSheep()
    {
        if (GameController.Instance.IsCageFull() == false)
        {
            int sheepType = Random.Range(1, ConfigManager.Instance.GetSheepMaxAtLevel(DataManager.Instance.PlayerData.userLevel));
            SheepController sheepController = SheepFactory.Instance.CreateNewSheep(sheepType);
            GameController.Instance.PutSheepBackToCage(sheepController);
            SimpleResourcesManager.Instance.ShowParticle("MergeFx", sheepController.transform.position, 1);
        }
    }
}

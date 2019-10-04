using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepFactory : Singleton<SheepFactory>
{
    Dictionary<int, GameObject> sheepPrototype=new Dictionary<int, GameObject>();
    Dictionary<int, List<SheepController>> dynamicSheepPool = new Dictionary<int, List<SheepController>>();


    public SheepController CreateNewSheep(int sheepType)
    {
        if (sheepPrototype.ContainsKey(sheepType))
        {
            List<SheepController> currentSheepPools = dynamicSheepPool[sheepType];
         
            int currentSheepPoolsCount = currentSheepPools.Count;
            for (int i=0;i<currentSheepPoolsCount;i++)
            {
                if (currentSheepPools[i].gameObject.activeSelf == false)
                {
                    currentSheepPools[i].gameObject.SetActive(true);
                    return currentSheepPools[i];
                }
            }

            GameObject sheepObj = Instantiate(sheepPrototype[sheepType]);
            SheepController sheepController = sheepObj.GetComponent<SheepController>();

            dynamicSheepPool[sheepType].Add(sheepController);
            return sheepObj.GetComponent<SheepController>();
        }
        else
        {
            GameObject sheepPref = Resources.Load<GameObject>(string.Format("Prefabs/Sheeps/Sheep{0}", sheepType));
            sheepPrototype.Add(sheepType, sheepPref);
            dynamicSheepPool.Add(sheepType, new List<SheepController>());


            GameObject sheepObj = Instantiate(sheepPref);
            SheepController sheepController = sheepObj.GetComponent<SheepController>();
            dynamicSheepPool[sheepType].Add(sheepController);
            return sheepController;
        }
    }


}

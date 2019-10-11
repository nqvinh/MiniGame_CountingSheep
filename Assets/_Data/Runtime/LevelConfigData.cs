using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class LevelConfigData
{
  [SerializeField]
  int level;
  public int Level { get {return level; } set { level = value;} }
  
  [SerializeField]
  long maxexp;
  public long Maxexp { get {return maxexp; } set { maxexp = value;} }
  
  [SerializeField]
  string[] reward_list = new string[0];
  public string[] Reward_List { get {return reward_list; } set { reward_list = value;} }
  
  [SerializeField]
  int maxsheep;
  public int Maxsheep { get {return maxsheep; } set { maxsheep = value;} }
  
}
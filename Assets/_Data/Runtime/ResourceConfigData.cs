using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class ResourceConfigData
{
  [SerializeField]
  string resourcesname;
  public string Resourcesname { get {return resourcesname; } set { resourcesname = value;} }
  
  [SerializeField]
  ResourceType resourcetype;
  public ResourceType RESOURCETYPE { get {return resourcetype; } set { resourcetype = value;} }
  
  [SerializeField]
  string resourcepath;
  public string Resourcepath { get {return resourcepath; } set { resourcepath = value;} }
  
  [SerializeField]
  int preloadamount;
  public int Preloadamount { get {return preloadamount; } set { preloadamount = value;} }
  
}
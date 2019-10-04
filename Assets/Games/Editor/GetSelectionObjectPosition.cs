using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetSelectionObjectPosition : Editor
{
    //Ctr "% Shift# P
    [MenuItem("Tools/GetTransformPosition/PrintPositions %#P")]
    private static void GenerateCode()
    {

        GameObject[] selectObj = Selection.gameObjects;

        int selectCount = selectObj.Length;
        string output = "Vector3[] waypoint = new Vector3[]{";
        for (int i = 0; i < selectCount; ++i)
        {
            //Vector3[] wayPoint = new Vector3[]{}
            output += "new Vector3(" + selectObj[i].transform.position.x+"f," + selectObj[i].transform.position.y + "f," + selectObj[i].transform.position.z + "f)";
            if (i < selectCount -1)
                output += ",";
           
        }
        output += "};";
        Debug.Log("Output: " + output);

    }
}

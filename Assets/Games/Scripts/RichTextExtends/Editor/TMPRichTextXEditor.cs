

using System;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TMPRichTextX))]
[CanEditMultipleObjects]
public class TMPRichTextXEditor : Editor {
  
    SerializedProperty text;
    SerializedProperty spriteAsset;
    
    private void OnEnable()
    {
        text = serializedObject.FindProperty("text");
        spriteAsset = serializedObject.FindProperty("spriteAsset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        text.stringValue = EditorGUILayout.TextArea(text.stringValue, GUILayout.Height(45));
        EditorGUILayout.PropertyField(spriteAsset);
        serializedObject.ApplyModifiedProperties();
       
        TMPRichTextX myTarget = (TMPRichTextX)target;
        myTarget.DoRenderRichText();
    }

   
}

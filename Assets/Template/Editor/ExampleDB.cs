using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExampleDB : MonoBehaviour
{

    List<ScriptableObject> objects;

    [ContextMenu("Generate")]
    public void GenerateFromEditor() {
    #if UNITY_EDITOR
        objects = new List<ScriptableObject>();
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foreach (string s in guids) {
            UnityEngine.Object o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(s));

            if (o is ScriptableObject) {
                objects.Add((ScriptableObject)o);
            }
        }

        EditorUtility.SetDirty(this);
    #endif
    }
}

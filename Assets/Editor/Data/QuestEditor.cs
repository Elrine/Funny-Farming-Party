using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quest), true)]
public class QuestEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Quest data = (Quest) target;

        
        if (GUILayout.Button("Update step"))
        {
            data.Init(data.data, null);
        }
    }
}

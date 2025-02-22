﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdatebleData), true)]
public class UpdatebleDataEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        UpdatebleData data = (UpdatebleData) target;

        
        if (GUILayout.Button("Update"))
        {
            data.NotifyOfUpdatedValues();
        }
    }
}

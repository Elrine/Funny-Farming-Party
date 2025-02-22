﻿using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.drawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.drawMapInEditor();
        }
        if (GUILayout.Button("Save noise"))
        {
            mapGen.saveNoise();
        }
    }
}

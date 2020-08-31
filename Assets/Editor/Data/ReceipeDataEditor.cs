using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (ReceipeData))]
public class ReceipeDataEditor : Editor {
    public override void OnInspectorGUI () {
        serializedObject.Update();
        ReceipeData receipe = (ReceipeData) target;

        EditorGUILayout.PropertyField (serializedObject.FindProperty ("type"));
        Debug.LogFormat("Type {0}", receipe.type.ToString());
        switch (receipe.type) {
            case ReceipeData.ReceipeType.item:
                EditorGUILayout.PropertyField (serializedObject.FindProperty ("itemSource"));
                break;
            case ReceipeData.ReceipeType.attribute:
                EditorGUILayout.PropertyField (serializedObject.FindProperty ("attributes"));
                break;
        }
        EditorGUILayout.PropertyField (serializedObject.FindProperty ("result"));
        serializedObject.ApplyModifiedProperties();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (QuestData))]
public class QuestDataEditor : Editor {
    bool showReward = false;
    bool[] showRewards;
    bool showStep = false;
    bool[] showSteps;

    public override void OnInspectorGUI () {
        serializedObject.Update ();

        QuestData quest = target as QuestData;

        initBool (quest);
        if (showRewards == null || showRewards.Length != quest.rewards.Length) {
            bool[] oldShowReward = showRewards;
            for (int i = quest.rewards.Length; oldShowReward != null && i < oldShowReward.Length; i++) {
                oldShowReward[i] = false;
            }
            showRewards = new bool[quest.rewards.Length];
            for (int i = 0; i < quest.rewards.Length; i++) {
                if (oldShowReward != null && i < oldShowReward.Length) {
                    showRewards[i] = oldShowReward[i];
                } else {
                    showRewards[i] = false;
                }
            }
        }

        if (showSteps == null || showSteps.Length != quest.steps.Length) {
            bool[] oldShowStep = showSteps;
            for (int i = quest.steps.Length; oldShowStep != null && i < oldShowStep.Length; i++) {
                oldShowStep[i] = false;
            }
            showSteps = new bool[quest.steps.Length];
            for (int i = 0; i < quest.steps.Length; i++) {
                if (oldShowStep != null && i < oldShowStep.Length) {
                    showSteps[i] = oldShowStep[i];
                } else {
                    showSteps[i] = false;
                }
            }
        }

        EditorGUILayout.PropertyField (serializedObject.FindProperty ("questName"));
        EditorGUILayout.PropertyField (serializedObject.FindProperty ("questDescription"));
        ShowRewardList (quest);
        ShowStepList (quest);

        serializedObject.ApplyModifiedProperties ();
    }

    void initBool (QuestData quest) {

    }

    void ShowRewardList (QuestData quest) {
        showReward = EditorGUILayout.Foldout (showReward, "Rewards");
        
        if (showReward) {EditorGUI.indentLevel += 1;
            for (int i = 0; i < serializedObject.FindProperty ("rewards.Array.size").intValue &&
                i < quest.rewards.Length; i++) {
                EditorGUILayout.BeginHorizontal ("Box");
                EditorGUILayout.BeginVertical ();
                showRewards[i] = EditorGUILayout.Foldout (showRewards[i], string.Format ("Reward {0}", i));
                if (showRewards[i]) {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField (serializedObject.FindProperty ("rewards.Array").GetArrayElementAtIndex (i).FindPropertyRelative ("type"));
                    switch (quest.rewards[i].type) {
                        case RewardData.RewardType.Item:
                            EditorGUILayout.PropertyField (serializedObject.FindProperty ("rewards.Array").GetArrayElementAtIndex (i).FindPropertyRelative ("item"));
                            break;
                        case RewardData.RewardType.Gold:
                            EditorGUILayout.PropertyField (serializedObject.FindProperty ("rewards.Array").GetArrayElementAtIndex (i).FindPropertyRelative ("gold"));
                            break;
                    }
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical ();
                ShowButtons (serializedObject.FindProperty ("rewards.Array"), i, ref showRewards);
                EditorGUILayout.EndHorizontal ();
            }
        EditorGUI.indentLevel -= 1;
        GUIContent addReward = new GUIContent ("+", "add reward");
            if (GUILayout.Button(addReward)) {
                serializedObject.FindProperty ("rewards.Array").InsertArrayElementAtIndex (0);
            }
        }
    }

    void ShowStepList (QuestData quest) {
        showStep = EditorGUILayout.Foldout (showStep, "Steps");
        if (showStep) {
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < quest.steps.Length &&
                i < serializedObject.FindProperty ("steps.Array.size").intValue; i++) {
                EditorGUILayout.BeginHorizontal ("Box");
                EditorGUILayout.BeginVertical ();
                showSteps[i] = EditorGUILayout.Foldout (showSteps[i], string.Format ("Step {0}", i));
                if (showSteps[i]) {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField (serializedObject.FindProperty ("steps.Array").GetArrayElementAtIndex (i).FindPropertyRelative ("type"));
                    switch (quest.steps[i].type) {
                        case StepData.StepType.Collect:
                            EditorGUILayout.PropertyField (serializedObject.FindProperty ("steps.Array").GetArrayElementAtIndex (i).FindPropertyRelative ("collectParams"), true);
                            break;
                        case StepData.StepType.Kill:
                            EditorGUILayout.PropertyField (serializedObject.FindProperty ("steps.Array").GetArrayElementAtIndex (i).FindPropertyRelative ("killParams"), true);
                            break;
                        case StepData.StepType.Talk:
                            EditorGUILayout.PropertyField (serializedObject.FindProperty ("steps.Array").GetArrayElementAtIndex (i).FindPropertyRelative ("talkParams"), true);
                            break;
                    }
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical ();
                ShowButtons (serializedObject.FindProperty ("steps.Array"), i, ref showSteps);
                EditorGUILayout.EndHorizontal ();
            }
            EditorGUI.indentLevel -= 1;
            GUIContent addStep = new GUIContent ("+", "add step");
            if (GUILayout.Button(addStep)) {
                serializedObject.FindProperty ("steps.Array").InsertArrayElementAtIndex (0);
            }
        }
    }

    public void ShowButtons (SerializedProperty list, int index, ref bool[] boolList) {
        GUILayoutOption miniButtonWidth = GUILayout.Width (20f);
        GUIContent moveDownButtonContent = new GUIContent ("\u21A7", "move down"),
            moveUpButtonContent = new GUIContent ("\u21A5", "move up"),
            duplicateButtonContent = new GUIContent ("+", "duplicate"),
            deleteButtonContent = new GUIContent ("-", "delete");
        if (GUILayout.Button (moveUpButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth)) {
            list.MoveArrayElement (index, index - 1);
            bool temp = boolList[index];
            boolList[index] = boolList[index - 1];
            boolList[index - 1] = temp;
        }
        if (GUILayout.Button (moveDownButtonContent, EditorStyles.miniButtonMid, miniButtonWidth)) {
            list.MoveArrayElement (index, index + 1);
            bool temp = boolList[index];
            boolList[index] = boolList[index + 1];
            boolList[index + 1] = temp;
        }
        if (GUILayout.Button (duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth)) {
            list.InsertArrayElementAtIndex (index);
        }
        if (GUILayout.Button (deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth)) {
            list.DeleteArrayElementAtIndex (index);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {
    public static List<Quest> playerQuest = new List<Quest> ();

    public static void addQuest (QuestData quest) {
        Debug.LogFormat("Add quest name '{0}'", quest.questName);
        Quest newQuest = ScriptableObject.CreateInstance<Quest> ();
        newQuest.Init (quest, Rewarding);
        playerQuest.Add(newQuest);
    }

    public static void Rewarding (Quest quest) {
        foreach (var reward in quest.data.rewards) {
            switch (reward.type) {
                case RewardData.RewardType.Gold:
                    InventoryPlayer.Instance.Gold += reward.gold;
                    break;
                case RewardData.RewardType.Item:
                    InventoryPlayer.Instance.AddItems (reward.item);
                    break;
                case RewardData.RewardType.LaunchQuest:
                    addQuest (reward.nextQuest);
                    break;
            }
        }
        playerQuest.Remove (quest);
    }
    
    public static void Kill (string target) {
        for (int i = 0; i < playerQuest.Count; i++)
        {
            playerQuest[i].Kill(target);
        }
    }

    public static void Collect (ItemData item) {
        for (int i = 0; i < playerQuest.Count; i++)
        {
            playerQuest[i].Collect(item);
        }
    }

    public static void Talk (string target) {
        for (int i = 0; i < playerQuest.Count; i++)
        {
            playerQuest[i].Talk(target);
        }
    }

    public static List<SavableQuest> ToSavableData() {
        List<SavableQuest> ToSave = new List<SavableQuest>();
        foreach (var quest in playerQuest)
        {
            ToSave.Add(quest.ToSavableData());
        }
        return ToSave;
    }

    public static void SetSavableData(List<SavableQuest> savableQuests) {
        playerQuest.Clear();
        foreach (var quest in savableQuests)
        {
            Quest newQuest = ScriptableObject.CreateInstance<Quest> ();
            newQuest.Init(quest);
            playerQuest.Add(newQuest);
        }
    }
}
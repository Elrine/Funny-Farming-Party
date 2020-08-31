using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "QuestData", menuName = "CustomData/Quest/QuestData", order = 0)]
public class QuestData : ScriptableObject {
    public string questName;
    [SerializeField]
    [TextArea]
    public string questDescription;
    public RewardData[] rewards;
    public StepData[] steps;
}

[System.Serializable]
public class RewardData {
    public enum RewardType {
        Item,
        Gold,
        LaunchQuest
    }
    public RewardType type;
    public int gold;
    public ItemStack item;
    public QuestData nextQuest;
}

[System.Serializable]
public class StepData  {
    public enum StepType {
        Collect,
        Kill,
        Talk
    }
    public StepType type;
    [System.Serializable]
    public class CollectParams {
        public ItemData item;
        public int needed;
    }
    public CollectParams collectParams;
    [System.Serializable]
    public class KillParams {
        public string target;
        public int needed;
    }
    public KillParams killParams;
    [System.Serializable]
    public class TalkParams {
        public string target;
    }
    public TalkParams talkParams;
}

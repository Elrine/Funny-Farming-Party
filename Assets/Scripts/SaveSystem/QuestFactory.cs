using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFactory : MonoBehaviour {
    [SerializeField]
    List<QuestData> listQuest = new List<QuestData>();
    private static QuestFactory _instance = null;
    public static QuestFactory Instance {
        get {
            return _instance;
        }
    }

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (this);
        }
    }
    public QuestData MakeQuest (SavableQuest quest) {
        foreach (var _quest in listQuest)
        {
            if (_quest.questName == quest.questName) {
                return _quest;
            }
        }
        return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDebugger : MonoBehaviour {
    [SerializeField]
    List<QuestData> questList = new List<QuestData> ();
    public bool debug = false;
    public static bool firstTime = true;

    // Start is called before the first frame update
    void Start () {
        if (firstTime) {
            if (debug) {
                foreach (var item in questList) {
                    QuestManager.addQuest (item);
                }
            }
            firstTime = false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepUI : MonoBehaviour {
    [SerializeField]
    private QuestStep data;

    public Text stepDescription = null;
    public GameObject stepComplete = null;

    public void SetStep (QuestStep _data) {
        data = _data;
        switch (data.data.type) {
            case StepData.StepType.Collect:
                stepDescription.text = $"Collect {data.data.collectParams.needed} {data.data.collectParams.item.itemName}\t{data.collected}/{data.data.collectParams.needed}";
                break;
            case StepData.StepType.Kill:
                stepDescription.text = $"Kill {data.data.killParams.needed} {data.data.killParams.target}\t{data.killed}/{data.data.killParams.needed}";
                break;
            case StepData.StepType.Talk:
                stepDescription.text = $"Talk to {data.data.talkParams.target}";
                break;
        }
        stepComplete.SetActive(data.isDone);
    }
}
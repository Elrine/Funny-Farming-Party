using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Quest", order = 0)]
public class Quest : ScriptableObject {
    public QuestData data;
    public int currentStepIndex;
    public QuestStep CurrentStep {
        get {
            return stepList[currentStepIndex];
        }
    }
    public List<QuestStep> stepList = new List<QuestStep> ();
    public System.Action<Quest> onFinishAction = null;

    public void Init (QuestData _data, System.Action<Quest> _onFinish) {
        data = _data;
        stepList.Clear();
        for (int i = 0; i < data.steps.Length; i++) {
            QuestStep newStep = new QuestStep();
            if (i + 1 < data.steps.Length)
                newStep.Init (data.steps[i], NextStep);
            else
                newStep.Init (data.steps[i], OnFinish);
            stepList.Add (newStep);
        }
        currentStepIndex = 0;
        onFinishAction += _onFinish;
    }

    public void Init(SavableQuest quest) {
        data = QuestFactory.Instance.MakeQuest(quest);
        currentStepIndex = quest.currentStepIndex;
        stepList.Capacity = data.steps.Length;
        for (int i = 0; i < data.steps.Length; i++) {
            QuestStep newStep = new QuestStep();
            if (i + 1 < data.steps.Length)
                newStep.Init (data.steps[i], quest.steps.Find((step) => step.stepIndex == i), NextStep);
            else
                newStep.Init (data.steps[i], quest.steps.Find((step) => step.stepIndex == i), OnFinish);
            stepList.Add(newStep);
        }
    }

    public void NextStep () {
        currentStepIndex++;
    }

    public void OnFinish () {
        onFinishAction (this);
    }
    public void Kill (string target) {
        stepList[currentStepIndex].Kill(target);
    }

    public void Collect (ItemData item) {
        stepList[currentStepIndex].Collect(item);
    }

    public void Talk (string target) {
        
        stepList[currentStepIndex].Talk(target);
    }

    public SavableQuest ToSavableData() {
        SavableQuest toSave = new SavableQuest();
        toSave.questName = data.questName;
        toSave.currentStepIndex = currentStepIndex;
        toSave.steps = new List<SavableStep>();
        for (int i = 0; i < stepList.Count; i++)
        {
            SavableStep step = new SavableStep();
            step.stepIndex = i;
            step.collected = stepList[i].collected;
            step.killed = stepList[i].killed;
            step.isDone = stepList[i].isDone;
            toSave.steps.Add(step);
        }
        return toSave;
    }
}

[System.Serializable]
public class SavableQuest {
    public string questName;
    public int currentStepIndex;
    public List<SavableStep> steps;
}

[System.Serializable]
public class SavableStep {
    public int stepIndex;
    public int collected;
    public int killed;
    public bool isDone;
}

[System.Serializable]
public class QuestStep {
    public StepData data;
    public int collected;
    public int killed;
    public bool isDone = false;
    public System.Action onFinishAction;
    public void Init (StepData _data, System.Action _onFinish) {
        data = _data;
        collected = 0;
        killed = 0;
        onFinishAction += _onFinish;
    }
    
    public void Init (StepData _data, SavableStep step, System.Action _onFinish) {
        data = _data;
        collected = step.collected;
        killed = step.killed;
        isDone = step.isDone;
        onFinishAction += _onFinish;
    }

    public void Kill (string target) {
        if (data.type == StepData.StepType.Kill && data.killParams.target == target) {
            killed++;
            if (killed >= data.killParams.needed) {
                onFinish ();
            }
        }
    }

    public void Collect (ItemData item) {
        if (data.type == StepData.StepType.Collect && data.collectParams.item == item) {
            collected++;
            if (collected >= data.collectParams.needed) {
                onFinish ();
            }
        }
    }

    public void Talk (string target) {
        if (data.type == StepData.StepType.Talk && data.killParams.target == target) {
            onFinish ();
        }
    }

    public void onFinish () {
        isDone = true;
        onFinishAction ();
    }
}
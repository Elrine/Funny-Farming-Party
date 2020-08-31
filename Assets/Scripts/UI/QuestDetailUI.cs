using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailUI : MonoBehaviour {
    [SerializeField]
    private Quest data;

    public Text titleText = null;
    public Text descriptionText = null;
    public RectTransform scrollStep = null;
    public RectTransform scrollReward = null;
    public GameObject stepPrefab = null;
    public GameObject rewardPrefab = null;

    private void Start () {
        if (data != null) {
            SetQuest (data);
        }
    }

    public void SetQuest (Quest _quest) {
        data = _quest;
        ClearQuest ();
        if (data == null) {
            return;
        }
        titleText.text = data.data.questName;
        descriptionText.text = data.data.questDescription;
        scrollStep.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 40 + data.stepList.Count * stepPrefab.GetComponent<RectTransform> ().rect.height);
        scrollReward.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 40 + data.data.rewards.Length * rewardPrefab.GetComponent<RectTransform> ().rect.width);
        foreach (var step in data.stepList) {
            GameObject stepInstance = GameObject.Instantiate (stepPrefab, scrollStep);

            StepUI stepUI = stepInstance.GetComponent<StepUI> ();
            stepUI.SetStep (step);
        }
        foreach (var reward in data.data.rewards) {

            if (reward.type != RewardData.RewardType.LaunchQuest) {
                GameObject rewardInstance = GameObject.Instantiate (rewardPrefab, scrollReward);
                RewardUI rewardUI = rewardInstance.GetComponent<RewardUI> ();
                rewardUI.SetReward (reward);
            }
        }
    }

    public void ClearQuest () {
        titleText.text = "";
        descriptionText.text = "";
        foreach (var step in scrollStep.GetComponentsInChildren<StepUI> ()) {
            Destroy (step.gameObject);
        }
        foreach (var reward in scrollReward.GetComponentsInChildren<RewardUI> ()) {
            Destroy (reward.gameObject);
        }
    }
}
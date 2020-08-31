using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour {
    [SerializeField]
    private RewardData data = null;
    public Text rewardName = null;
    public Text rewardNumber = null;
    public RawImage rewardImage = null;
    [SerializeField]
    private Texture2D goldImage = null;


    public void SetReward (RewardData _data) {
        data = _data;
        UpdateReward();
    }

    public void UpdateReward() {
        rewardName.gameObject.SetActive (true);
        rewardNumber.gameObject.SetActive (true);
        rewardImage.gameObject.SetActive (true);
        switch (data.type) {
            case RewardData.RewardType.Item:
                rewardName.text = data.item.data.itemName;
                rewardNumber.text = data.item.sizeStack.ToString ();
                rewardImage.texture = data.item.data.itemInHUD;
                break;
            case RewardData.RewardType.Gold:
                rewardName.text = "Gold";
                rewardNumber.text = data.gold.ToString ();
                rewardImage.texture = goldImage;
                break;
            default:
                rewardName.gameObject.SetActive (false);
                rewardNumber.gameObject.SetActive (false);
                rewardImage.gameObject.SetActive (false);
                break;
        }
    }

    private void OnValidate() {
        if (data != null) {
            if (data.type == RewardData.RewardType.Item &&
            data.item.data != null &&
            data.item.sizeStack > 0) {
                UpdateReward();
            } else if (data.type == RewardData.RewardType.Gold && data.gold > 0) {
                UpdateReward();
            }
        }
    }
}
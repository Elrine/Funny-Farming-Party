using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSelectorUI : MonoBehaviour
{
    public Quest data = null;
    public QuestUI controler = null;
    [SerializeField]
    private Button button = null;

    public void Init(Quest _data, QuestUI _controler) {
        data = _data;
        controler = _controler;
        button.GetComponentInChildren<Text>().text = data.data.questName;
        SetSelected(false);
    }

    public void SetSelected(bool isSelected) {
        button.GetComponent<Image>().enabled = isSelected;
    }

    public void OnSelected() {
        controler.SetCurrentSelector(this);
        SetSelected(true);
    }
}

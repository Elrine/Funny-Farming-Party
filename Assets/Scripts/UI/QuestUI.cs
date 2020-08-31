using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class QuestUI : MonoBehaviour {
    [SerializeField]
    public List<Quest> debugQuestList = new List<Quest> ();
    public QuestDetailUI questDetails = null;
    public List<QuestSelectorUI> listQuest = new List<QuestSelectorUI> ();
    public RectTransform scrollQuest = null;
    public GameObject background = null;
    public GameObject selectorPrefab = null;
    public QuestSelectorUI currentSelector = null;
    public bool isVisible = false;
    public bool debug = false;
    private static QuestUI _instance;
    public QuestUI Instance {
        get {
            return _instance;
        }
    }

    public void Awake () {
        if (_instance != null) {
            Destroy (gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start () {
        SetVisible (isVisible);
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.J)) {
            SetVisible (!isVisible);
        }
    }

    public void SetVisible (bool _visiblity) {
        isVisible = _visiblity;
        background.SetActive (isVisible);
        if (isVisible) {
            UpdateQuests (debug ? debugQuestList : QuestManager.playerQuest);
        }
        RigidbodyFirstPersonController player = FindObjectOfType<RigidbodyFirstPersonController> ();
        if (player != null) {
            player.enabled = !isVisible;
            player.mouseLook.SetCursorLock (!isVisible);
        }
    }

    public void UpdateQuests (List<Quest> _listQuest) {
        foreach (var quest in listQuest) {
            Destroy (quest.gameObject);
        }
        listQuest.Clear ();
        scrollQuest.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, _listQuest.Count * selectorPrefab.GetComponent<RectTransform> ().rect.height);
        foreach (var quest in _listQuest) {
            GameObject newInstance = Instantiate (selectorPrefab, scrollQuest);
            QuestSelectorUI newSelector = newInstance.GetComponent<QuestSelectorUI> ();
            newSelector.Init (quest, this);
            listQuest.Add (newSelector);
        }
        questDetails.ClearQuest();
        if (listQuest.Count > 0)
            listQuest[0].OnSelected ();
    }

    public void SetCurrentSelector (QuestSelectorUI newCurrentSelector) {
        foreach (var quest in listQuest) {
            quest.SetSelected (false);
        }
        questDetails.SetQuest (newCurrentSelector.data);
        currentSelector = newCurrentSelector;
    }
}
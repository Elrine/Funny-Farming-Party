using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    private static SaveManager _instance = null;
    public static SaveManager Instance {
        get {
            return _instance;
        }
    }

    private float timeout = -1;

    public string environementName = null;
    public EnvironementData env = null;
    SaveData saveObject = null;

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            SaveSystem.Init ();
            DontDestroyOnLoad (this);
        } else {
            Destroy (this);
        }
    }

    private void updateEnv () {
        GameObject player = GameObject.FindWithTag ("Player");
        env.playerPosition = player.transform.position;
        env.playerRotation = player.transform.rotation;
        env.playerScene = Loader.getCurrentScene ();
        env.currentTime = DayNightCycle.Instance.currentTime;
        env.isCreated = true;
    }

    public void SaveWorld () {
        SaveData saveObject = new SaveData ();
        updateEnv ();
        saveObject.env = env;
        saveObject.listRessource = RessouceGenerator.Instance.ToSavableData ();
        saveObject.inventoryPlayer = InventoryPlayer.Inventory.ToSavableData ();
        saveObject.questPlayer = QuestManager.ToSavableData ();
        string data = JsonUtility.ToJson (saveObject);
        SaveSystem.Save (environementName, data);
    }

    private IEnumerator setEnv (EnvironementData newEnv) {
        env = newEnv;
        Teleport.disableTeleport = true;
        Loader.LoadScrene (env.playerScene);
        yield return new WaitWhile (() => Loader.getCurrentScene () != env.playerScene);
        GameObject player = null;
        while (player == null) {
            player = GameObject.FindWithTag ("Player");
            yield return new WaitForFixedUpdate ();
        }
        player.transform.SetPositionAndRotation (env.playerPosition, env.playerRotation);
        DayNightCycle.Instance.currentTime = env.currentTime;
        Teleport.disableTeleport = false;
        if (env.playerScene == Loader.Scene.OuterWorld)
            StartCoroutine (RessouceGenerator.Instance.placeSavedRessource ());
    }

    public void createWorld () {
        env = new EnvironementData();
        Loader.LoadScrene (Loader.Scene.OuterWorld);
    }

    public void LoadWorld () {
        string data = SaveSystem.Load (environementName);
        if (data != null) {
            saveObject = JsonUtility.FromJson<SaveData> (data);
            Loader.Scene currentScene = Loader.getCurrentScene ();
            StartCoroutine (setEnv (saveObject.env));
            RessouceGenerator.listSavedRessource = saveObject.listRessource;
            InventoryPlayer.setSavedInventory (saveObject.inventoryPlayer);
            QuestManager.SetSavableData (saveObject.questPlayer);
        }
    }

    private void Update () {
        if (timeout >= 0) {
            timeout += Time.unscaledDeltaTime;
            Debug.LogFormat ("timeout:{0}, instance of inventory: {1}, should finish wait {2}", timeout, InventoryPlayer.Instance != null, InventoryPlayer.Instance != null || timeout > 120);
        }
    }

    public void changeSeed (float seed) {
        env.seed = Mathf.FloorToInt (seed);
    }

    public void changeWorldName (string value) {
        environementName = value;
    }
}

public class SaveData {
    public EnvironementData env;
    public List<SavableRessource> listRessource;
    public SavableInventoryData inventoryPlayer;
    public List<SavableQuest> questPlayer;
}
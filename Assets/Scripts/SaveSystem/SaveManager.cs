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
    }

    public void SaveWorld () {
        SaveData saveObject = new SaveData ();
        updateEnv ();
        saveObject.env = env;
        saveObject.listRessource = RessouceGenerator.Instance.ToSavableData ();
        saveObject.inventoryPlayer = InventoryPlayer.Inventory.ToSavableData ();
        string data = JsonUtility.ToJson (saveObject);
        SaveSystem.Save (environementName, data);
    }

    private IEnumerator setEnv (EnvironementData newEnv) {
        env = newEnv;
        if (Loader.getCurrentScene () != env.playerScene) {
            Loader.LoadScrene (env.playerScene);
            yield return new WaitWhile (() => Loader.getCurrentScene () != env.playerScene);
        }
        GameObject player = null;
        while (player == null) {
            player = GameObject.FindWithTag ("Player");
            yield return new WaitForFixedUpdate ();
        }
        player.transform.SetPositionAndRotation (env.playerPosition, env.playerRotation);
        DayNightCycle.Instance.currentTime = env.currentTime;
    }

    public IEnumerator LoadWorld () {
        SaveData saveObject = new SaveData ();
        string data = SaveSystem.Load (environementName);
        if (data != null) {
            saveObject = JsonUtility.FromJson<SaveData> (data);
            StartCoroutine (setEnv (saveObject.env));
            timeout = 0;
            yield return new WaitUntil (() => RessouceGenerator.Instance != null || timeout > 120);
            timeout = -1;
            if (RessouceGenerator.Instance == null)
                Debug.LogError ("Ressource Generator not found");
            else {
                RessouceGenerator.Instance.listSavedRessource.Clear ();
                RessouceGenerator.Instance.listSavedRessource = saveObject.listRessource;
                if (saveObject.env.playerScene == Loader.Scene.OuterWorld) {
                    StartCoroutine (RessouceGenerator.Instance.placeSavedRessource ());
                }
                timeout = 0;
                yield return new WaitUntil (() => InventoryPlayer.Instance != null || timeout > 120);
                timeout = -1;
                if (InventoryPlayer.Instance != null) {
                    InventoryPlayer.Instance.setSavedInventory (saveObject.inventoryPlayer);
                }
            }
        }
    }

    private void Update () {
        if (timeout >= 0)
            timeout += Time.unscaledDeltaTime;
        if (Input.GetKeyDown (KeyCode.M)) {
            SaveWorld ();
        }
        if (Input.GetKeyDown (KeyCode.L)) {
            StartCoroutine (LoadWorld ());
        }
    }
}

public class SaveData {
    public EnvironementData env;
    public List<SavableRessource> listRessource;
    public SavableInventoryData inventoryPlayer;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceFactory : MonoBehaviour {
    [System.Serializable]
    public class RessourceTypeRow {
        public RessourceData.RessourceType type;
        public RessourceData[] ModelList;
    }
    public List<RessourceTypeRow> ressourceType = new List<RessourceTypeRow> ();
    private static RessourceFactory _instance = null;
    public static RessourceFactory Instance {
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

    public RessourceData MakeRessource (SavableRessource ressource) {
        foreach (var item in ressourceType.Find ((_ressource) => _ressource.type == ressource.type).ModelList) {
            if (item.ressourceName == ressource.data.ressourceName) {
                return item;
            }
        }
        return null;
    }
}
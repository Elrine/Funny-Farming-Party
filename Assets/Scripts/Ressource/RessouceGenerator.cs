using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessouceGenerator : MonoBehaviour {
    private static RessouceGenerator _instance = null;
    public static RessouceGenerator Instance {
        get {
            return _instance;
        }
    }
    private Dictionary<Vector2, RessourceData> listRessource = new Dictionary<Vector2, RessourceData> ();
    private const int ressourceSizeMax = 3;

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad (this.gameObject);
        } else {
            Destroy (this);
        }
    }

    public bool placeValid (Vector2 pos, RessourceData toPlace) {
        Vector2 posInGrid = pos;
        if (listRessource.ContainsKey (posInGrid)) {
            return false;
        }
        return true;
    }

    public GameObject placeRessource (Vector3 pos, RessourceData toPlace) {
        Vector2 posInGrid = new Vector2 (pos.x, pos.z);
        if (placeValid (posInGrid, toPlace)) {
            listRessource.Add (posInGrid, toPlace);
            return GameObject.Instantiate (toPlace.prefab, pos, Quaternion.identity);
        } else {
            return null;
        }
    }

    public void removeRessource (Vector2 pos) {
        listRessource.Remove (pos);
    }
}
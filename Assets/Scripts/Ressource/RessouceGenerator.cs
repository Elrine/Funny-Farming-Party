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
    public List<Ressource> listRessource = new List<Ressource> ();
    public List<SavableRessource> listSavedRessource = new List<SavableRessource> ();
    public int sqrShowDistance = 250;
    private const int ressourceSizeMax = 3;
    static Dictionary<InfiniteTerrain.TerrainChunk, Queue<SavableRessource>> savableRessourceOfChunk = new Dictionary<InfiniteTerrain.TerrainChunk, Queue<SavableRessource>> ();

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad (this.gameObject);
        } else {
            Destroy (this);
        }
    }

    public bool placeValid (Vector2 pos, RessourceData toPlace) {
        Vector3 posPortal = SaveManager.Instance.env.portalPosition;
        Vector2 posPortalInGrid = new Vector2 (posPortal.x, posPortal.z);
        if (listRessource.Exists (ressource => ressource.pos == pos) || (pos - posPortalInGrid).sqrMagnitude < 4) {
            return false;
        }
        return true;
    }

    public GameObject placeRessource (Vector3 pos, RessourceData toPlace) {
        Vector2 posInGrid = new Vector2 (pos.x, pos.z);
        if (placeValid (posInGrid, toPlace)) {
            GameObject placed = GameObject.Instantiate (toPlace.prefab, pos, Quaternion.identity);
            if (placed != null) {
                Ressource newRessource = new Ressource (posInGrid, placed.GetComponent<RessourceAbstact> ());
                listRessource.Add (newRessource);
            }
            return placed;
        } else {
            return null;
        }
    }

    public IEnumerator onColliderRecivedCoroutine (InfiniteTerrain.TerrainChunk chunk) {
        while (savableRessourceOfChunk[chunk].Count > 0) {
            SavableRessource ressource = savableRessourceOfChunk[chunk].Dequeue ();
            Ray ray = new Ray (new Vector3 (ressource.pos.x, 100, ressource.pos.y), Vector3.down);
            RaycastHit[] hitList = Physics.RaycastAll (ray, 100);
            Debug.DrawRay (ray.origin, ray.direction * 100, Color.blue, 10);
            for (int i = 0; hitList.Length == 0 && i < 10; i++) {
                yield return new WaitForSeconds (.1f);
                hitList = Physics.RaycastAll (ray, 100);
                Debug.DrawRay (ray.origin, ray.direction * 100, Color.blue, 10);
            }
            foreach (var hit in hitList) {
                if (hit.collider.CompareTag ("Chunk")) {
                    GameObject placed = placeRessource (hit.point, RessourceFactory.Instance.MakeRessource (ressource));
                    switch (ressource.type) {
                        case RessourceData.RessourceType.PlantType:
                            if (placed != null)
                                placed.GetComponent<Plant> ().CurrentGrowth = ressource.plantData.currentGrowth;
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }
        }
        savableRessourceOfChunk.Remove (chunk);
    }

    public void onColliderRecived (bool immediateCall, InfiniteTerrain.TerrainChunk chunk) {
        StartCoroutine (onColliderRecivedCoroutine (chunk));
        if (!immediateCall)
            chunk.unsubscribleRequestCollider (onColliderRecived);
    }

    public IEnumerator placeSavedRessource () {
        Vector2 viewerPos = InfiniteTerrain._viewerPos;
        clearRessource ();
        InfiniteTerrain terrainManager = FindObjectOfType<InfiniteTerrain> ();
        for (int i = 0; terrainManager == null && i < 10; i++) {
            yield return new WaitForSeconds (.1f);
            terrainManager = FindObjectOfType<InfiniteTerrain> ();
        }
        foreach (var item in listSavedRessource) {
            InfiniteTerrain.TerrainChunk chunk = terrainManager.getChunkOfPoint (item.pos);
            for (int i = 0; chunk == null && i < 10; i++) {
                yield return new WaitForSeconds (.1f);
                chunk = terrainManager.getChunkOfPoint (item.pos);
            }
            if (savableRessourceOfChunk.ContainsKey (chunk)) {
                savableRessourceOfChunk[chunk].Enqueue (item);
            } else {
                savableRessourceOfChunk.Add (chunk, new Queue<SavableRessource> ());
                savableRessourceOfChunk[chunk].Enqueue (item);
                chunk.subscribeRequestCollider (onColliderRecived);
            }
        }
    }

    public void clearRessource () {
        foreach (var ressource in listRessource) {
            Destroy (ressource.ressource.gameObject);
        }
        listRessource.Clear ();
    }

    public void removeRessource (Vector2 pos) {
        listRessource.RemoveAll (ressource => ressource.pos == pos);
    }

    public List<SavableRessource> ToSavableData () {
        List<SavableRessource> toSave = new List<SavableRessource> ();
        foreach (var item in listRessource) {
            SavableRessource toSaveRessource = new SavableRessource ();
            toSaveRessource.pos = item.pos;
            toSaveRessource.type = item.ressource.ressourceType.GetRessourceType;
            switch (toSaveRessource.type) {
                case RessourceData.RessourceType.PlantType:
                    toSaveRessource.plantData = item.ressource.ToSavableData () as SavablePlant;
                    break;
                default:
                    break;
            }
            toSaveRessource.data = item.ressource.ToSavableData ().data;
            toSave.Add (toSaveRessource);
        }
        foreach (var savedRessource in listSavedRessource) {
            if (!toSave.Exists (ressource => savedRessource.pos == ressource.pos)) {
                toSave.Add (savedRessource);
            }
        }
        return toSave;
    }

    public void saveRessource () {
        listSavedRessource = ToSavableData ();
    }
}

public class Ressource {
    public Vector2 pos;
    public RessourceAbstact ressource;

    public Ressource (Vector2 _pos, RessourceAbstact _ressource) {
        pos = _pos;
        ressource = _ressource;
    }
}

[System.Serializable]
public class SavableRessource {
    public Vector2 pos;
    public RessourceData.RessourceType type;
    public SavablePlant plantData;
    public SavableRessourceData data;
}
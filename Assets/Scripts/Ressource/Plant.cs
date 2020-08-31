using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : RessourceAbstact {
    [SerializeField]
    private SeedData plantType = null;
    public SeedData PlantType {
        get {
            return plantType;
        }
    }

    [SerializeField]
    [Range (0, 1)]
    private float currentGrowth = 0f;
    public float CurrentGrowth {
        get {
            return currentGrowth;
        }
        set {
            currentGrowth = value;
            if (currentGrowth > 1) {
                currentGrowth = 1;
                isMature = true;
            }
        }
    }

    [SerializeField]
    private float scale = 1f;
    [SerializeField]
    private DayNightCycle clock;
    [SerializeField]
    private bool showGrow = true;
    [SerializeField]
    private Transform modelPlant;
    private Transform modelSeed;
    public float lastUpdate = -1;
    private bool createModel = false;
    // Start is called before the first frame update
    void Start () {
        createModel = true;
        if (clock == null) {
            clock = FindObjectOfType<DayNightCycle> ();
        }
        SetModel ();
        isMature = false;
        if (ressourceType == null) {
            ressourceType = plantType.seedOf;
        }
    }

    // Update is called once per frame
    void Update () {
        Grow ();
        ScaleToGrow ();
    }

    void Grow () {
        if (currentGrowth < 1) {
            float deltaTime;
            if (lastUpdate == -1)
                deltaTime = clock.getDeltaTime ();
            else {
                deltaTime = clock.getDeltaTime (lastUpdate);
                lastUpdate = -1;
            }
            CurrentGrowth += deltaTime / plantType.seedOf.daysToGrow;
        } else if (!isMature) {
            isMature = true;
        }
    }

    public void AccelerateGrow(float delta) {
        CurrentGrowth += delta / plantType.seedOf.daysToGrow;
    }

    void ScaleToGrow () {
        if (modelPlant != null)
            modelPlant.localScale = Vector3.one * currentGrowth * scale;
        if (modelSeed != null)
            modelSeed.localScale = Vector3.one - (Vector3.one * currentGrowth * scale);
    }

    private void OnValidate () {
        SetModel ();
        if (showGrow)
            ScaleToGrow ();
        else {
            modelPlant.localScale = Vector3.one * scale;
            modelSeed.localScale = Vector3.zero;
        }
    }

    void SetModel () {
        if (modelPlant == null || modelSeed == null) {
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform> ();
            foreach (var _transform in transforms) {
                if (_transform.gameObject.tag == "Plant" && _transform != transform) {
                    modelPlant = _transform;
                }
                if (_transform.gameObject.tag == "Seed") {
                    modelSeed = _transform;
                }
            }
            if (createModel) {
                if (modelPlant == null) {
                    GameObject plant;
                    if (ressourceType.prefab != null) {
                        plant = GameObject.Instantiate (ressourceType.prefab, transform.position, Quaternion.identity, transform);
                    } else {
                        plant = GameObject.CreatePrimitive (PrimitiveType.Sphere);
                        plant.transform.parent = transform;
                        plant.transform.localPosition = Vector3.zero;
                    }
                    modelPlant = plant.transform;
                    plant.tag = "Plant";
                }
                if (modelSeed == null) {
                    GameObject seed;
                    if (plantType.itemInWorld != null) {
                        seed = GameObject.Instantiate (plantType.itemInWorld, transform.position, Quaternion.identity, transform);
                    } else {
                        seed = GameObject.CreatePrimitive (PrimitiveType.Sphere);
                        seed.transform.parent = transform;
                        seed.transform.localPosition = Vector3.zero;
                    }
                    modelSeed = seed.transform;
                    seed.tag = "Seed";
                }
            }
        }
    }

    protected override void OnHarvesting () {
        RessouceGenerator.Instance.removeRessource (posInGrid);
        GameObject.Destroy (gameObject);
    }

    public void SetVisble (bool visible) {
        if (!visible) {
            lastUpdate = Time.time;
        }
        gameObject.SetActive (visible);
    }

    public override SavableRessourceAbstact ToSavableData () {
        SavablePlant toSave = new SavablePlant ();
        Grow ();
        toSave.data = ressourceType.ToSavableData ();
        toSave.currentGrowth = currentGrowth;
        return toSave;
    }
}

[System.Serializable]
public class SavablePlant : SavableRessourceAbstact {
    public float currentGrowth;
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {
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
    }

    [SerializeField]
    private float scale = 1f;
    [SerializeField]
    private DayNightCycle clock;
    [SerializeField]
    private bool showGrow = true;
    private Transform modelPlant;
    private Transform modelSeed;
    [SerializeField]
    private bool createModel = false;
    // Start is called before the first frame update
    void Start () {
        createModel = true;
        if (clock == null) {
            clock = FindObjectOfType<DayNightCycle> ();
        }
        SetModel ();
    }

    // Update is called once per frame
    void Update () {
        Grow ();
        ScaleToGrow ();
    }

    void Grow () {
        if (currentGrowth < 1) {
            float deltaTime = clock.getDeltaTime ();
            currentGrowth += deltaTime / plantType.seedOf.daysToGrow;
            if (currentGrowth > 1) {
                currentGrowth = 1;
            }
        }
    }

    void ScaleToGrow () {
        modelPlant.localScale = Vector3.one * currentGrowth * scale;
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
        if (createModel) {
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform> ();
            foreach (var _transform in transforms) {
                if (_transform.gameObject.tag == "Plant") {
                    modelPlant = _transform;
                }
                if (_transform.gameObject.tag == "Seed") {
                    modelSeed = _transform;
                }
            }
            if (modelPlant == null) {
                GameObject plant;
                if (plantType.seedOf.prefab != null) {
                    plant = GameObject.Instantiate (plantType.seedOf.prefab, transform.position, Quaternion.identity, transform);
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

    private void OnDestroy () {
        RessourceData.Drop[] dropList = plantType.seedOf.dropItem;
        Array.Sort (dropList, new RessourceData.Drop.SortByDropRate ());
        System.Random pgrn = new System.Random ((int) DateTime.Now.Ticks);
        float percent = pgrn.Next (0, 10000) / 10000f;
        foreach (var drop in dropList) {
            if (percent <= drop.dropRate) {
                int numberOfDrop = Mathf.RoundToInt (drop.numberDrop.Evaluate (percent * drop.dropRate) * (drop.maxDrop - drop.minDrop)) + drop.minDrop;
                for (int i = 0; i < numberOfDrop; i++) {
                    GameObject.Instantiate (drop.item.itemInWorld, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
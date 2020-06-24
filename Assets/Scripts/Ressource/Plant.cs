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
    }

    [SerializeField]
    private float scale = 1f;
    [SerializeField]
    private DayNightCycle clock;
    [SerializeField]
    private bool showGrow = true;
    private Transform modelPlant;
    private Transform modelSeed;
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
            float deltaTime = clock.getDeltaTime ();
            currentGrowth += deltaTime / plantType.seedOf.daysToGrow;
            if (currentGrowth > 1) {
                currentGrowth = 1;
                isMature = true;
            }
        } else if (!isMature) {
            isMature = true;
        }
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
                if (_transform.gameObject.tag == "Plant") {
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

    protected override void OnHarvesting()
    {
        GameObject.Destroy(gameObject);
    }
}
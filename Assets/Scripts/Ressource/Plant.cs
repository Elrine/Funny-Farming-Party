using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Plant : MonoBehaviour {
    [SerializeField]
    private PlantData plantType = null;
    public PlantData PlantType {
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
    public GameObject model;
    // Start is called before the first frame update
    void Start () {
        if (clock == null) {
            clock = FindObjectOfType<DayNightCycle> ();
        }
        SetModel();
    }

    // Update is called once per frame
    void Update () {
        Grow ();
        ScaleToGrow ();
    }

    void Grow () {
        if (currentGrowth < 1) {
            float deltaTime = clock.getDeltaTime ();
            currentGrowth += deltaTime / plantType.daysToGrow;
            if (currentGrowth > 1) {
                currentGrowth = 1;
            }
        }
    }

    void ScaleToGrow () {
        model.transform.localScale = Vector3.one * currentGrowth * scale;
    }

    private void OnValidate () {
        SetModel();
        if (showGrow)
            ScaleToGrow ();
        else
            model.transform.localScale = Vector3.one * scale;
    }

    void SetModel() {
        if (model == null)
            model = gameObject;
    }

    private void OnDestroy() {
        RessourceData.Drop[] dropList = plantType.dropItem;
        Array.Sort(dropList, new RessourceData.Drop.SortByDropRate());
        System.Random pgrn = new System.Random((int)DateTime.Now.Ticks);
        float percent = pgrn.Next(0, 10000) / 10000f;
        foreach (var drop in dropList)
        {
            if (percent <= drop.dropRate) {
                int numberOfDrop = Mathf.RoundToInt(drop.numberDrop.Evaluate(percent * drop.dropRate) * (drop.maxDrop - drop.minDrop)) + drop.minDrop;
                for (int i = 0; i < numberOfDrop; i++)
                {
                    GameObject.Instantiate(drop.item.itemInWorld, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
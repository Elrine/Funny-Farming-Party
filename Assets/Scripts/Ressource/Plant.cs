using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {
    [SerializeField]
    private PlantData plantType = null;
    [SerializeField]
    [Range (0, 1)]
    private float currentGrowth = 0f;
    [SerializeField]
    private float scale = 1f;
    [SerializeField]
    private DayNightCycle clock;
    [SerializeField]
    private bool showGrow = true;
    // Start is called before the first frame update
    void Start () {
        if (clock == null) {
            clock = FindObjectOfType<DayNightCycle> ();
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
            currentGrowth += deltaTime / plantType.timeToGrow;
            if (currentGrowth > 1) {
                currentGrowth = 1;
            }
        }
    }

    void ScaleToGrow () {
        transform.localScale = Vector3.one * currentGrowth * scale;
    }

    private void OnValidate () {
        if (showGrow)
            ScaleToGrow ();
        else
            transform.localScale = Vector3.one;
    }
}
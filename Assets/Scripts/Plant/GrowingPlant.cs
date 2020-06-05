using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingPlant : MonoBehaviour {
    public MeshRenderer stem;
    public Color color;
    [Range (0, 1)]
    public float GrowPercent;
    public Shader shader;
    Material material;

    private void Awake () { }

    private void OnValidate () {
        updateMaterial ();
    }

    public void updateMaterial () {
        ApplyToMaterial ();
    }

    void ApplyToMaterial () {
        if (material == null)
            material = new Material (shader);
        material.SetColor ("_Color", color);
        this.GetComponentsInChildren<MeshRenderer>();
        material.SetFloat ("_GrowPercent", GrowPercent);
        if (stem != null)
            stem.sharedMaterial = material;
    }
}
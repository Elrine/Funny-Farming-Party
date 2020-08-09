using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu ()]
public class TextureData : UpdatebleData {
    public Region[] regions;

    float savedMinHeight;
    float savedMaxHeight;
    float savedWaterHeight = -1;
    public float minHeight {
        get {
            return savedMinHeight;
        }
    }
    public float maxHeight {
        get {
            return savedMaxHeight;
        }
    }
    public float waterHeight {
        get {
            return savedWaterHeight;
        }
    }

    public void ApplyToMaterial (Material material) {
        material.SetInt ("regionCount", regions.Length);
        for (int i = 0; i < regions.Length; i++) {
            material.SetColor ("Color" + i, regions[i].baseColor);
            material.SetFloat ("Height" + i, regions[i].baseStartHeight);
        }

        updateMeshHeights (material, savedMinHeight, savedMaxHeight);
    }

    public void updateMeshHeights (Material material, float minHeight, float maxHeight) {
        this.savedMinHeight = minHeight;
        this.savedMaxHeight = maxHeight;

        foreach (var region in regions) {
            if (region.isWaterLevel) {
                this.savedWaterHeight = (savedMaxHeight - savedMinHeight) * region.baseStartHeight + savedMinHeight - 1;
                break;
            }
        }
        material.SetFloat ("MinHeight", minHeight - 1);
        material.SetFloat ("MaxHeight", maxHeight);
    }

    [System.Serializable]
    public class Region {
        public string name;
        public Color baseColor;
        [Range (0, 1)]
        public float baseStartHeight;
        [Range (0, 1)]
        public float baseBlend;
        public List<string> plantVaild;
        public bool isWaterLevel;
    }
}
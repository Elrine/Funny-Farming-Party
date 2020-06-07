using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu ()]
public class TextureData : UpdatebleData {
    public Region[] regions;

    float savedMinHeight;
    float savedMaxHeight;
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

    public void ApplyToMaterial (Material material) {
        material.SetInt ("regionCount", regions.Length);
        material.SetColorArray ("baseColors", regions.Select(x => x.baseColor).ToArray());
        material.SetFloatArray ("baseStartHeights", regions.Select(x => x.baseStartHeight).ToArray());
        material.SetFloatArray ("baseBlends", regions.Select(x => x.baseBlend).ToArray());

        updateMeshHeights (material, savedMinHeight, savedMaxHeight);
    }

    public void updateMeshHeights (Material material, float minHeight, float maxHeight) {
        this.savedMinHeight = minHeight;
        this.savedMaxHeight = maxHeight;

        material.SetFloat ("minHeight", minHeight - 1);
        material.SetFloat ("maxHeight", maxHeight);
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
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatebleData
{
    public Color[] baseColors;
    [Range(0,1)]
    public float[] baseStartHeights;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material) {
        material.SetInt("baseColorCount", baseColors.Length);
        material.SetColorArray("baseColors", baseColors);
        material.SetFloatArray("baseStartHeights", baseStartHeights);

        updateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void updateMeshHeights(Material material, float minHeight, float maxHeight) {
        this.savedMinHeight = minHeight;
        this.savedMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}

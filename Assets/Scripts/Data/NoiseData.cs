using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatebleData
{
    public Noise.NormalizeMode normalizeMode;
    public int seed;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persitance;
    public float lacunarity;
    public Vector2 offset;

    protected override void OnValidate() {
        if (lacunarity < 1) {
            lacunarity = 1;
        }
        if (octaves < 0) {
            octaves = 0;
        }

        base.OnValidate();
    }
}

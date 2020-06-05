using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    public enum NormalizeMode {
        Local,
        Global
    }

    public static float[, ] generateNoiseMap (
        int mapWidth,
        int mapHeight,
        int seed,
        float scale,
        int octaves,
        float persitance,
        float lacunarity,
        Vector2 offset,
        NormalizeMode normalize) {
        float[, ] newMap = new float[mapWidth, mapHeight];

        float maxPossibleHeight = 0;

        System.Random prng = new System.Random (seed);
        Vector2[] octavesOffsets = new Vector2[octaves];

        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next (-10000, 10000) + offset.x;
            float offsetY = prng.Next (-10000, 10000) - offset.y;
            octavesOffsets[i] = new Vector2 (offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= persitance;
        }

        if (scale <= 0) {
            scale = .0001f;
        }

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        float halfHeight = mapHeight / 2f;
        float halfWidth = mapWidth / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth + octavesOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octavesOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persitance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxHeight) {
                    maxHeight = noiseHeight;
                } else if (noiseHeight < minHeight) {
                    minHeight = noiseHeight;
                }
                newMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                if (normalize == NormalizeMode.Local)
                    newMap[x, y] = Mathf.InverseLerp (minHeight, maxHeight, newMap[x, y]);
                else {
                    float normalizedHeight = (newMap[x,y] + 1) / (maxPossibleHeight);
                    newMap[x,y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue );
                }
            }
        }
        return newMap;
    }
}
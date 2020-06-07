using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public enum DrawMode {
        NoiseMap,
        NoiseMesh,
        Mesh
    }
    public DrawMode drawMode;

    public NoiseData heightMapSettings;
    public TerrainData terrainSettings;
    public TextureData textureSettings;

    public Material terrainMaterial;

    public const int mapChunkSize = 239;
    [Range (0, 6)]
    public int editorPreviewLOD;

    public bool autoUpdate;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>> ();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>> ();

    public void drawMapInEditor () {
        MapData map = generateMapData (Vector2.zero, false);

        MapDisplay display = FindObjectOfType<MapDisplay> ();
        switch (drawMode) {
            case DrawMode.NoiseMap:
                display.drawMap (TextureGenerator.generateNoiseTexture (map.heightMap));
                break;
            case DrawMode.Mesh:
                display.drawMesh (MeshGenerator.generateTerrainMesh (map.heightMap, terrainSettings.meshHeightMultiplier, terrainSettings.meshHeightCurve, editorPreviewLOD));
                break;
            case DrawMode.NoiseMesh:
                display.drawTextureMesh (MeshGenerator.generateTerrainMesh (map.heightMap, terrainSettings.meshHeightMultiplier, terrainSettings.meshHeightCurve, editorPreviewLOD), TextureGenerator.generateNoiseTexture(map.heightMap));
                break;
            default:
                break;
        }
    }

    public void OnValuesUpdated () {
        if (!Application.isPlaying) {
            drawMapInEditor ();
        }
    }
    public void OnTextureValuesUpdated () {
        textureSettings.ApplyToMaterial (terrainMaterial);
    }

    public MapData generateMapData (Vector2 center, bool inThread) {
        float[, ] noiseMap = Noise.generateNoiseMap (mapChunkSize + 2, mapChunkSize + 2, heightMapSettings.seed, heightMapSettings.noiseScale, heightMapSettings.octaves, heightMapSettings.persitance, heightMapSettings.lacunarity, heightMapSettings.offset + center, heightMapSettings.normalizeMode);

        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x, y];
            }
        }
        if (!inThread)
            textureSettings.updateMeshHeights (terrainMaterial, terrainSettings.minHeight, terrainSettings.maxHeight);
        return new MapData (noiseMap);
    }

    public void requestMapData (Vector2 center, Action<MapData> callback) {
        textureSettings.updateMeshHeights (terrainMaterial, terrainSettings.minHeight, terrainSettings.maxHeight);
        ThreadStart threadStart = delegate {
            mapDataThread (center, callback);
        };
        new Thread (threadStart).Start ();
    }

    public void mapDataThread (Vector2 center, Action<MapData> callback) {
        MapData mapData = generateMapData (center, true);
        lock (mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue (new MapThreadInfo<MapData> (callback, mapData));
        }
    }

    public void requestMeshData (MapData mapData, int lod, Action<MeshData> callback) {
        ThreadStart threadStart = delegate {
            meshDataThread (mapData, lod, callback);
        };
        new Thread (threadStart).Start ();
    }

    public void meshDataThread (MapData mapData, int lod, Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.generateTerrainMesh (mapData.heightMap, terrainSettings.meshHeightMultiplier, terrainSettings.meshHeightCurve, lod);
        lock (mapDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue (new MapThreadInfo<MeshData> (callback, meshData));
        }
    }

    private void OnValidate () {
        if (terrainSettings != null) {
            terrainSettings.onValuesUpdated -= OnValuesUpdated;
            terrainSettings.onValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null) {
            heightMapSettings.onValuesUpdated -= OnValuesUpdated;
            heightMapSettings.onValuesUpdated += OnValuesUpdated;
        }
        if (textureSettings != null) {
            textureSettings.onValuesUpdated -= OnTextureValuesUpdated;
            textureSettings.onValuesUpdated += OnTextureValuesUpdated;
        }
    }

    private void Update () {
        if (mapDataThreadInfoQueue.Count > 0) {
            while (mapDataThreadInfoQueue.Count > 0) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue ();
                threadInfo.callback (threadInfo.param);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0) {
            while (meshDataThreadInfoQueue.Count > 0) {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue ();
                threadInfo.callback (threadInfo.param);
            }
        }
    }

    private int mapNumber = 0;

    public void saveNoise () {
        float[, ] noiseMap = Noise.generateNoiseMap (mapChunkSize, mapChunkSize, heightMapSettings.seed, heightMapSettings.noiseScale, heightMapSettings.octaves, heightMapSettings.persitance, heightMapSettings.lacunarity, heightMapSettings.offset, Noise.NormalizeMode.Local);
        byte[] data = TextureGenerator.generateNoiseTexture (noiseMap).EncodeToPNG ();
        System.IO.File.WriteAllBytes ("./Assets/Ressources/noiseMap" + (mapNumber > 0 ? mapNumber.ToString () : "") + ".png", data);
        mapNumber++;
    }

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T param;
        public MapThreadInfo (Action<T> callback, T param) {
            this.callback = callback;
            this.param = param;
        }
    }
}

public struct MapData {
    public float[, ] heightMap;

    public MapData (float[, ] heightMap) {
        this.heightMap = heightMap;
    }
}
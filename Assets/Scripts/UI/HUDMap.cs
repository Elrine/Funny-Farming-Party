using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDMap : MonoBehaviour
{
    public InfiniteTerrain terrainGenerator;
    // Update is called once per frame
    void Update()
    {
        RawImage image = gameObject.GetComponent<RawImage>();
        InfiniteTerrain.TerrainChunk chunk = terrainGenerator.getCurrentChunk();
        if (chunk == null || !chunk.mapReceived) {
            return;
        }
        MapData map = chunk.map;
        Texture2D texture = TextureGenerator.generateNoiseTexture(map.heightMap);
        Vector2 viewerPos = terrainGenerator.viewerPos;
        Vector2 chunkPos = chunk.pos;
        Vector2 topLeftChunkPos = new Vector2(chunkPos.x + (map.heightMap.GetLength(0) / 2), chunkPos.y - (map.heightMap.GetLength(0) / 2));
        Vector2 viewerPosOnChunk = new Vector2(viewerPos.x + topLeftChunkPos.x, viewerPos.y - topLeftChunkPos.y);
        texture.SetPixel(Mathf.RoundToInt(viewerPosOnChunk.x), Mathf.RoundToInt(viewerPosOnChunk.y), Color.red);
        texture.Apply();
        image.texture = texture;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour {

    const float viewerThesholdToUpdate = 25f;
    const float sqrViewerThesholdToUpdate = viewerThesholdToUpdate * viewerThesholdToUpdate;
    public LODInfo[] detailLevels;
    public static float maxViewDst;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 _viewerPos;
    public Vector2 viewerPos {
        get {
            return _viewerPos;
        }
    }
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunkVisibleInViewDis;
    Dictionary<Vector2, TerrainChunk> chunkDictionary = new Dictionary<Vector2, TerrainChunk> ();
    static List<TerrainChunk> chunksVisibleLastUpdate = new List<TerrainChunk> ();
    Vector2 oldViewerPos;

    private IEnumerator Start () {
        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        mapGenerator = FindObjectOfType<MapGenerator> ();
        yield return new WaitWhile (() => viewer == null);
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunkVisibleInViewDis = Mathf.RoundToInt (maxViewDst / chunkSize);
        UpdateVisibleChunk ();
    }

    private void Update () {
        if (viewer == null) {
            GameObject player = GameObject.FindWithTag ("Player");
            if (player != null)
            viewer = player.transform;
        } else {
            _viewerPos = new Vector2 (viewer.position.x, viewer.position.z) / mapGenerator.terrainSettings.uniformScale;
            if ((oldViewerPos - _viewerPos).sqrMagnitude > sqrViewerThesholdToUpdate) {
                oldViewerPos = _viewerPos;
                UpdateVisibleChunk ();
            }
        }
    }

    void UpdateVisibleChunk () {
        foreach (TerrainChunk chunk in chunksVisibleLastUpdate) {
            chunk.setVisible (false);
        }
        chunksVisibleLastUpdate.Clear ();

        int currentChunkOffsetX = Mathf.RoundToInt (_viewerPos.x / chunkSize);
        int currentChunkOffsetY = Mathf.RoundToInt (_viewerPos.y / chunkSize);

        for (int yOffset = -chunkVisibleInViewDis; yOffset <= chunkVisibleInViewDis; yOffset++) {
            for (int xOffset = -chunkVisibleInViewDis; xOffset <= chunkVisibleInViewDis; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2 (currentChunkOffsetX + xOffset, currentChunkOffsetY + yOffset);
                if (chunkDictionary.ContainsKey (viewedChunkCoord)) {
                    chunkDictionary[viewedChunkCoord].Update ();
                } else {
                    chunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public Vector2 worldPosToChunkPos (Vector3 position) {
        return new Vector2 (position.x, position.z) / mapGenerator.terrainSettings.uniformScale;
    }

    public TerrainChunk getChunkOfPoint (Vector2 pos) {
        int currentChunkOffsetX = Mathf.RoundToInt (pos.x / chunkSize);
        int currentChunkOffsetY = Mathf.RoundToInt (pos.y / chunkSize);

        Vector2 viewedChunkCoord = new Vector2 (currentChunkOffsetX, currentChunkOffsetY);
        if (chunkDictionary.ContainsKey (viewedChunkCoord)) {
            return chunkDictionary[viewedChunkCoord];
        } else {
            return null;
        }
    }

    public TerrainChunk getCurrentChunk () {
        int currentChunkOffsetX = Mathf.RoundToInt (_viewerPos.x / chunkSize);
        int currentChunkOffsetY = Mathf.RoundToInt (_viewerPos.y / chunkSize);

        Vector2 viewedChunkCoord = new Vector2 (currentChunkOffsetX, currentChunkOffsetY);
        if (chunkDictionary.ContainsKey (viewedChunkCoord)) {
            return chunkDictionary[viewedChunkCoord];
        } else {
            return null;
        }
    }

    private void OnDestroy () {
        chunksVisibleLastUpdate.Clear ();
    }

    public class TerrainChunk {
        GameObject meshObject;
        public Vector2 pos;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLODMesh;
        System.Action<bool, TerrainChunk> onColliderRecived = null;

        MapData mapData;
        bool mapDataReceived;
        public bool mapReceived {
            get {
                return mapDataReceived;
            }
        }
        int previousLODIndex = -1;
        public MapData map {
            get {
                return mapData;
            }
        }

        public TerrainChunk (Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material) {
            this.detailLevels = detailLevels;
            pos = coord * size;
            bounds = new Bounds (pos, Vector2.one * size);
            Vector3 posV3 = new Vector3 (pos.x, 0, pos.y);

            meshObject = new GameObject ("Chunk");
            meshObject.tag = "Chunk";
            meshRenderer = meshObject.AddComponent<MeshRenderer> ();
            meshFilter = meshObject.AddComponent<MeshFilter> ();
            meshCollider = meshObject.AddComponent<MeshCollider> ();
            meshRenderer.material = material;

            meshObject.transform.position = posV3 * mapGenerator.terrainSettings.uniformScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapGenerator.terrainSettings.uniformScale;
            setVisible (false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < lodMeshes.Length; i++) {
                lodMeshes[i] = new LODMesh (detailLevels[i].lod, Update);
                if (detailLevels[i].useForCollider) {
                    collisionLODMesh = lodMeshes[i];
                }
            }
            mapGenerator.requestMapData (pos, onMapDataReceived);
        }

        public void subscribeRequestCollider (System.Action<bool, TerrainChunk> callback) {
            if (collisionLODMesh.hasMesh && meshObject.activeSelf) {
                callback (true, this);
            } else
                onColliderRecived += callback;
        }

        public void unsubscribleRequestCollider (System.Action<bool, TerrainChunk> callback) {
            onColliderRecived -= callback;
        }

        public void Update () {
            if (mapDataReceived) {
                float viewerDstFromEdge = Mathf.Sqrt (bounds.SqrDistance (_viewerPos));
                bool visible = viewerDstFromEdge <= maxViewDst;

                if (visible) {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++) {
                        if (viewerDstFromEdge > detailLevels[i].visibleDstThreshold) {
                            lodIndex++;
                        } else {
                            break;
                        }
                    }
                    if (lodIndex != previousLODIndex) {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh) {
                            meshFilter.mesh = lodMesh.mesh;
                            previousLODIndex = lodIndex;
                        } else if (!lodMesh.hasRequestedMesh) {
                            lodMesh.requestMesh (mapData);
                        }
                    }
                    if (lodIndex == 0) {
                        if (collisionLODMesh.hasMesh) {
                            meshCollider.sharedMesh = collisionLODMesh.mesh;
                            if (onColliderRecived != null)
                                onColliderRecived (false, this);
                        } else if (!collisionLODMesh.hasRequestedMesh) {
                            collisionLODMesh.requestMesh (mapData);
                        }
                    }

                    chunksVisibleLastUpdate.Add (this);
                }
                setVisible (visible);
            }
        }

        void onMapDataReceived (MapData mapData) {
            this.mapData = mapData;
            mapDataReceived = true;

            Update ();
        }

        public void setVisible (bool visible) {
            meshObject.SetActive (visible);
            if (onColliderRecived != null && collisionLODMesh != null && collisionLODMesh.hasMesh)
                onColliderRecived (false, this);
        }

        public bool isVisible () {
            return meshObject.activeSelf;
        }
    }

    class LODMesh {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh (int lod, System.Action updateCallback) {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void onReceivedMeshData (MeshData meshData) {
            mesh = meshData.createMesh ();
            hasMesh = true;
            updateCallback ();
        }

        public void requestMesh (MapData mapData) {
            hasRequestedMesh = true;
            mapGenerator.requestMeshData (mapData, lod, onReceivedMeshData);
        }
    }

    [System.Serializable]
    public struct LODInfo {
        public int lod;
        public float visibleDstThreshold;
        public bool useForCollider;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtHighlight : MonoBehaviour {
    public GameObject prefab;
    static MapGenerator mapGenerator;
    GameObject oldGameObject;
    public float distanceToHit;
    public Material transparantMaterial;
    public bool isActivate = true;

    private void Awake () {
        mapGenerator = FindObjectOfType<MapGenerator> ();
        if (prefab == null) {
            prefab = GameObject.CreatePrimitive (PrimitiveType.Sphere);
            MeshRenderer meshRenderer = prefab.AddComponent<MeshRenderer> ();
            meshRenderer.material = transparantMaterial;
        }
    }
    // Update is called once per frame
    void Update () {
        if (isActivate) {
            UpdateRaycast();
        }
    }

    void UpdateRaycast() {
        Transform cam = Camera.main.transform;
        Ray ray = new Ray (cam.position, cam.forward);
        RaycastHit hit;
        Debug.DrawRay (ray.origin, ray.direction * distanceToHit, Color.red);
        if (Physics.Raycast (ray.origin, ray.direction, out hit, distanceToHit)) {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null || !meshCollider.gameObject.CompareTag ("Chunk")) {
                if (oldGameObject != null) {
                    Object.Destroy (oldGameObject);
                    oldGameObject = null;
                }
                return;
            }
            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3[] listPoint = new Vector3[3];
            Transform hitTransform = hit.collider.transform;
            for (int i = 0; i < 3; i++) {
                listPoint[i] = vertices[triangles[hit.triangleIndex * 3 + i]];
                listPoint[i] = hitTransform.TransformPoint (listPoint[i]);
            }
            Debug.DrawLine (listPoint[0], listPoint[1]);
            Debug.DrawLine (listPoint[1], listPoint[2]);
            Debug.DrawLine (listPoint[2], listPoint[0]);
            Vector3 pos = nearestPoint (listPoint, hit.point);
            TextureData.Region currentRegion = findCurrentRegion (mapGenerator.textureSettings, pos);
            if (pos != Vector3.zero) {
                if (oldGameObject == null) {
                    oldGameObject = Object.Instantiate (prefab, pos, Quaternion.identity);
                    SetMaterials (oldGameObject);
                } else {
                    oldGameObject.transform.position = pos;
                }
            } else {
                Object.Destroy (oldGameObject);
                oldGameObject = null;
            }
        } else if (oldGameObject != null) {
            Object.Destroy (oldGameObject);
            oldGameObject = null;
        }
    }

    void SetMaterials (GameObject _gameObject) {
        MeshRenderer meshRenderer = _gameObject.GetComponent<MeshRenderer> ();
        if (meshRenderer == null) {
            MeshRenderer[] meshRenderers = _gameObject.GetComponentsInChildren<MeshRenderer> ();
            foreach (var renderer in meshRenderers) {
                Material[] materials = new Material[renderer.materials.GetLength (0)];
                for (int i = 0; i < materials.GetLength (0); i++) {
                    materials[i] = transparantMaterial;
                }
                renderer.materials = materials;
            }
        } else {
            Material[] materials = new Material[meshRenderer.materials.GetLength (0)];
            for (int i = 0; i < materials.GetLength (0); i++) {
                materials[i] = transparantMaterial;
            }
            meshRenderer.materials = materials;
        }
    }

    TextureData.Region findCurrentRegion (TextureData textureData, Vector3 pos) {
        int regionIndex = 0;
        float minHeight = textureData.minHeight;
        float maxHeight = textureData.maxHeight;
        float currentHeight = pos.y;
        float percentHeight = 0;
        while (regionIndex < textureData.regions.Length - 2) {
            percentHeight = (currentHeight - minHeight) / maxHeight;
            if (percentHeight < textureData.regions[regionIndex + 1].baseStartHeight && percentHeight >= textureData.regions[regionIndex].baseStartHeight) {
                return textureData.regions[regionIndex];
            } else {
                regionIndex++;
            }
        }
        if (percentHeight < textureData.regions[regionIndex + 1].baseStartHeight && percentHeight >= textureData.regions[regionIndex].baseStartHeight) {
            return textureData.regions[regionIndex];
        } else {
            return textureData.regions[regionIndex + 1];
        }
    }

    Vector3 nearestPoint (Vector3[] listVertex, Vector3 point) {
        float minDist = 1000;
        Vector3 nearestPoint = Vector3.zero;
        foreach (var vertex in listVertex) {
            float distSqr = (vertex - point).sqrMagnitude;
            if (distSqr < minDist) {
                nearestPoint = vertex;
                minDist = distSqr;
            }
        }
        return nearestPoint;
    }
}
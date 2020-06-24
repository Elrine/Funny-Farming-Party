using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour {
    private Camera cam;
    private RaycastHit hit;
    [SerializeField]
    private float distanceToPlace = 10f;
    [SerializeField]
    private GameObject indicatorPrefab = null;
    private Vector3 targetPos = Vector3.zero;
    private GameObject oldIndicator = null;

    private void Awake () {
        cam = Camera.main;
    }

    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (!InventoryPlayer.Instance.inventoryShown) {
            UpdateColision ();
            PlaceRessource ();
        }
    }

    public void PlaceRessource () {
        if (InventoryPlayer.Instance.getCurrentSlot () && InventoryPlayer.Instance.getCurrentSlot ().GetItemType == ItemData.ItemType.SeedType) {
            SeedData seed = InventoryPlayer.Instance.getCurrentSlot () as SeedData;
            Vector2 targetPosOnGrid = new Vector2 (targetPos.x, targetPos.z);
            if (targetPos != Vector3.zero && RessouceGenerator.Instance.placeValid (targetPosOnGrid, seed.seedOf)) {
                if (oldIndicator == null) {
                    if (indicatorPrefab == null)
                        oldIndicator = GameObject.CreatePrimitive (PrimitiveType.Sphere);
                    else
                        oldIndicator = GameObject.Instantiate (indicatorPrefab);
                    oldIndicator.transform.position = targetPos;
                    oldIndicator.transform.localScale = Vector3.one * seed.seedOf.sizeRessource;
                } else {
                    oldIndicator.transform.position = targetPos;
                }
                if (Input.GetKeyDown (KeyCode.Mouse0)) {
                    RessouceGenerator.Instance.placeRessource (targetPos, seed.seedOf);
                    InventoryPlayer.Instance.RemoveCurrentItem ();
                    GameObject.Destroy (oldIndicator);
                    oldIndicator = null;
                }
            } else {
                GameObject.Destroy (oldIndicator);
                oldIndicator = null;
            }
        } else {
            GameObject.Destroy (oldIndicator);
            oldIndicator = null;
        }
    }

    public void UpdateColision () {
        Ray ray = new Ray (cam.transform.position, cam.transform.forward);
        Debug.DrawRay (ray.origin, ray.direction * distanceToPlace, Color.red);
        RaycastHit[] allHit = Physics.RaycastAll (ray, distanceToPlace);
        bool hitChunk = false;
        bool hitRessource = false;
        InventoryPlayer.SlotType slotType = InventoryPlayer.Instance.getCurrentSlotType ();
        foreach (var _hit in allHit) {
            if (slotType == InventoryPlayer.SlotType.Plant && _hit.collider.gameObject.CompareTag ("Chunk")) {
                hitChunk = true;
                hit = _hit;
            }
            if (Input.GetMouseButtonDown (0) && slotType != InventoryPlayer.SlotType.Plant && _hit.collider.gameObject.CompareTag ("Plant")) {
                hitRessource = true;
                hit = _hit;
            }
        }
        if (hitChunk) {
            MeshCollider chunckCollider = hit.collider as MeshCollider;
            if (chunckCollider) {
                targetPos = FindPointInChunck (chunckCollider);
                Debug.DrawRay (targetPos, Vector3.up * 3, Color.green);
            } else
                targetPos = Vector3.zero;
        } else {
            targetPos = Vector3.zero;
            if (hitRessource) {
                BoxCollider ressourceCollider = hit.collider as BoxCollider;
                if (ressourceCollider) {
                    RessourceAbstact plant = ressourceCollider.GetComponent<RessourceAbstact> ();
                    plant.Harvest ();
                }
            }
        }
    }

    public Vector3 FindPointInChunck (MeshCollider chunk) {
        Mesh mesh = chunk.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] listPoint = new Vector3[3];
        Transform hitTransform = hit.collider.transform;
        for (int i = 0; i < 3; i++) {
            listPoint[i] = vertices[triangles[hit.triangleIndex * 3 + i]];
            listPoint[i] = hitTransform.TransformPoint (listPoint[i]);
        }
        return nearestPoint (listPoint, hit.point);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtInfo : MonoBehaviour {

    private Camera cam;
    private Plant targetPlant;
    private RaycastHit hit;
    [SerializeField]
    private HUDPlant hud = null;
    [SerializeField]
    private float distanceToInfo = 30f;
    [SerializeField]
    private Vector2 offsetHud = new Vector2();

    // Start is called before the first frame update
    void Start () {
        cam = Camera.main;
        hud.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update () {
        UpdateColision();
        UpdateHudPlant();
    }

    void UpdateColision () {
        Ray ray = new Ray (cam.transform.position, cam.transform.forward);
        Debug.DrawRay (ray.origin, ray.direction * distanceToInfo, Color.red);
        if (Physics.Raycast (ray, out hit, distanceToInfo)) {
            BoxCollider box = hit.collider as BoxCollider;
            targetPlant = box == null ? null : box.GetComponent<Plant> ();
        } else {
            targetPlant = null;
        }
    }

    void UpdateHudPlant () {
        if (targetPlant != null) {
            if (hud.target == null) {
                hud.gameObject.SetActive (true);
                hud.target = targetPlant;
            }
            Vector3 targetPos = targetPlant.transform.position;
            Vector3 pos = new Vector3(targetPos.x, hit.point.y, targetPos.z);
            Vector2 posOnScreen = cam.WorldToScreenPoint(pos);
            RectTransform hudTransform = hud.GetComponent<RectTransform>();
            hudTransform.position = posOnScreen + offsetHud;
        }
        else if (hud.target != null || hud.gameObject.activeSelf) {
            hud.target = null;
            hud.gameObject.SetActive(false);
        }
    }
}
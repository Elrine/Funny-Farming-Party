﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtInfo : MonoBehaviour {

    private Camera cam = null;
    private Plant targetPlant;
    private RaycastHit hit;
    [SerializeField]
    private float distanceToInfo = 30f;
    [SerializeField]
    private Vector2 offsetHud = new Vector2();

    // Start is called before the first frame update
    void Start () {
        cam = Camera.main;
        // HUDPlant.Instance.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update () {
        if (cam == null) {
            cam = Camera.main;
        } else {
        UpdateColision();
        UpdateHudPlant();
        }
    }

    void UpdateColision () {
        Ray ray = new Ray (cam.transform.position, cam.transform.forward);
        if (Physics.Raycast (ray, out hit, distanceToInfo)) {
            BoxCollider box = hit.collider as BoxCollider;
            targetPlant = box == null ? null : box.GetComponent<Plant> ();
        } else {
            targetPlant = null;
        }
    }

    void UpdateHudPlant () {
        if (targetPlant != null) {
            if (HUDPlant.Instance.target == null) {
                HUDPlant.Instance.gameObject.SetActive (true);
                HUDPlant.Instance.target = targetPlant;
            }
            Vector3 targetPos = targetPlant.transform.position;
            Vector3 pos = new Vector3(targetPos.x, hit.point.y, targetPos.z);
            Vector2 posOnScreen = cam.WorldToScreenPoint(pos);
            RectTransform hudTransform = HUDPlant.Instance.GetComponent<RectTransform>();
            hudTransform.position = posOnScreen + offsetHud;
        }
        else if (HUDPlant.Instance != null && (HUDPlant.Instance.target != null || HUDPlant.Instance.gameObject.activeSelf)) {
            HUDPlant.Instance.target = null;
            HUDPlant.Instance.gameObject.SetActive(false);
        }
    }
}
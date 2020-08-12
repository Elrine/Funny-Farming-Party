using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider))]
public class Teleport : MonoBehaviour {
    public Loader.Scene toScene = Loader.Scene.OuterWorld;

    private static bool isTeleported = true;

    // Start is called before the first frame update
    private void OnTriggerEnter (Collider other) {
        if (other.CompareTag ("Player")) {
            RessouceGenerator.Instance.saveRessource();
            RessouceGenerator.Instance.clearRessource();
            DayNightCycle.Instance.gameObject.GetComponentInChildren<Light>().enabled = toScene == Loader.Scene.OuterWorld;
            Loader.LoadScrene (toScene);
        }
    }

    private void Start () {
        TeleportResult();
    }

    private void Update() {
        if (isTeleported) {
            TeleportResult();
        }
    }

    private void TeleportResult() {
            GameObject player = GameObject.FindWithTag ("Player");
            if (player) {
                isTeleported = false;
                float angle = UnityEngine.Random.Range (-1000, 1000) * Mathf.PI / 1000;
                Vector3 offset = new Vector3 (Mathf.Cos (angle) , 1, Mathf.Sin (angle)) * 2;
                player.transform.SetPositionAndRotation(transform.position + offset, Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up));
                if (InventoryPlayer.Instance != null) {
                    InventoryPlayer.Instance.NotifyAll ();
                }
                if (Loader.getCurrentScene() == Loader.Scene.OuterWorld && RessouceGenerator.Instance != null) {
                    StartCoroutine(RessouceGenerator.Instance.placeSavedRessource());
                }
            }
    }
}
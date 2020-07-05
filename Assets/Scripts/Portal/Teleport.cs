using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider))]
public class Teleport : MonoBehaviour {
    [SerializeField]
    private Loader.Scene toScene = Loader.Scene.OuterWorld;

    private static bool isTeleported = false;

    // Start is called before the first frame update
    private void OnTriggerEnter (Collider other) {
        if (other.CompareTag ("Player")) {
            isTeleported = true;
            InventoryPlayer.Instance.isFixData = true;
            Loader.LoadScrene (toScene);
        }
    }

    private void Update () {
        if (isTeleported) {
            GameObject player = GameObject.FindWithTag ("Player");
            if (player) {
                Debug.Log ("Teleported!");
                isTeleported = false;
                float angle = Random.Range (-1000, 1000) / 1000f;
                Vector3 offset = new Vector3 (Mathf.Cos (angle), 1, Mathf.Sin (angle));
                player.transform.position = transform.position + offset;
                if (InventoryPlayer.Instance != null) {
                    InventoryPlayer.Instance.isFixData = false;
                    InventoryPlayer.Instance.NotifyAll ();
                    Debug.Log ("NotifyAll");
                }
            }
        }
    }
}
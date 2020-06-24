using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider))]
[RequireComponent (typeof (Rigidbody))]
public class Item : MonoBehaviour {
    [SerializeField]
    private ItemData item = null;
    private float timer = 0;
    [SerializeField]
    private float stayCheckDelay = 1;

    private void Awake () {
        
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.CompareTag ("Player")) {
            if (InventoryPlayer.Instance.AddItem (item))
                GameObject.Destroy (gameObject);
        }
    }

    private void OnTriggerStay (Collider other) {
        if (other.gameObject.CompareTag ("Player")) {
            timer = Time.deltaTime;
            if (timer >= stayCheckDelay) {
                timer = 0;
                if (InventoryPlayer.Instance.AddItem (item))
                    GameObject.Destroy (gameObject);
            }
        }
    }

    private void OnCollisionEnter (Collision other) {
        if (other.gameObject.CompareTag ("Chunk")) {
            Rigidbody rigidbody = GetComponent<Rigidbody> ();
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
        }
    }
}
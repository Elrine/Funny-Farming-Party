using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtShop : MonoBehaviour {
    public int distanceToSee = 10;

    void Update () {
        if (Input.GetMouseButton (0) && !InventoryPlayer.Instance.inventoryShown) {
            Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
            Debug.DrawRay (ray.origin, ray.direction, Color.black, 2.5f);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, distanceToSee)) {
                ShopControler shop = hit.collider.GetComponent<ShopControler> ();
                if (shop != null)
                    shop.showShop ();
            }
        }
    }
}
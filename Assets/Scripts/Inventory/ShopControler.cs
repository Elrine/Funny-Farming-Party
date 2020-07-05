using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopControler : MonoBehaviour {
    [SerializeField]
    private StoreData storeData = null;

    public void showShop () {
        if (!ShopInventory.Instance.Show) {
            ShopInventory.Instance.Show = true;
            ShopInventory.Instance.Data = Instantiate (storeData);
            InventoryPlayer.Instance.setUIVisible (true);
        }
    }

    private void Update () {
        if (ShopInventory.Instance.Show) {
            if (Input.GetKeyDown (KeyCode.I) || Input.GetKeyDown (KeyCode.Escape)) {
                ShopInventory.Instance.Show = false;
                InventoryPlayer.Instance.setUIVisible (false);
            }
        }
    }
}
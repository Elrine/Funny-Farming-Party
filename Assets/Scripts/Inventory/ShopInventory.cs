using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ShopInventory : IInventory {
    private static ShopInventory _instance;
    public static ShopInventory Instance {
        get {
            return _instance;
        }
    }
    private StoreData _data;
    public StoreData Data {
        get {
            return _data;
        }
        set {
            _data = value;
            NotifyUpdateAll ();
        }
    }

    [SerializeField]
    private int shopSize = 10;
    [SerializeField]
    private int slotByRow = 5;
    private bool _show;
    public GameObject UIShop;
    public bool Show {
        get {
            return _show;
        }
        set {
            _show = value;
            SlotUI slot;
            foreach (var ui in toHide) {
                slot = ui.GetComponent<SlotUI> ();
                if (slot) {
                    slot.showSlot (_show);
                } else {
                    ui.SetActive (_show);
                }
            }
            UIShop.SetActive (_show);
        }
    }
    public GameObject[] toHide;

    private List<Action<ItemStack>> listCallback = null;

    void NotifyUpdate (int index) {
        if (_show && _data && listCallback[index] != null) {
            listCallback[index] (index >= _data.sellItem.Count ? null : new ItemStack (_data.sellItem[index]));
        }
    }

    void NotifyUpdateAll () {
        for (int i = 0; i < listCallback.Count; i++) {
            NotifyUpdate (i);
        }
    }

    public override void RemoveSlot (Vector2 pos) {
        int index = Mathf.RoundToInt (pos.x + pos.y * slotByRow);
        if (_data.sellItem.Count > index && InventoryPlayer.Instance.Gold >= _data.sellItem[index].goldValue) {
            InventoryPlayer.Instance.Gold -= _data.sellItem[index].goldValue;
        } else {
            InventoryPlayer.Instance.RewindLastItem ();
        }
        NotifyUpdate (index);
    }

    public override bool SetItemAt (Vector2 pos, ItemStack stack) {
        int index = Mathf.RoundToInt (pos.x + pos.y * slotByRow);
        NotifyUpdate (index);
        if (_data.sellItem.Count > index)
            return false;
        InventoryPlayer.Instance.Gold += Mathf.RoundToInt (stack.data.goldValue * stack.sizeStack);
        return true;
    }

    public override void subscribeUpdate (Vector2 pos, Action<ItemStack> callback) {
        int index = Mathf.RoundToInt (pos.x + pos.y * slotByRow);
        listCallback[index] += callback;
        NotifyUpdate (index);
    }

    public override void unsubscribeUpdate (Vector2 pos, Action<ItemStack> callback) {
        if (listCallback == null) {
            listCallback = new List<Action<ItemStack>> ();
            for (int i = 0; i < shopSize; i++) {
                listCallback.Add (null);
            }
        } else {
            int index = Mathf.RoundToInt (pos.x + pos.y * slotByRow);
            listCallback[index] -= callback;
        }
    }

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            GameObject.DontDestroyOnLoad (transform.parent.gameObject);
            listCallback = new List<Action<ItemStack>> ();
            for (int i = 0; i < shopSize; i++) {
                listCallback.Add (null);
            }
        } else {
            Destroy (gameObject);
        }
    }

    private void Start () {
        Show = false;
    }
}
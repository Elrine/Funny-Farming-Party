using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class InventoryPlayer : MonoBehaviour {
    public uint sizeInventory = 15;
    public const uint sizeInventoryBar = 5;
    public Dictionary<Vector2, System.Action<ItemStack>> callbackList = new Dictionary<Vector2, System.Action<ItemStack>> ();
    public SelectorCursorUI cursor = null;
    public uint gold = 0;
    [SerializeField]
    const uint sizeRowInventory = 5;
    private Dictionary<Vector2, ItemStack> inventoryData = null;

    [SerializeField]
    private InventorySlot[] initInventoryData = null;
    private bool showInventory = false;
    private RigidbodyFirstPersonController player = null;
    [SerializeField]
    public GameObject background = null;

    public void subscribeUpdate (Vector2 pos, System.Action<ItemStack> callback) {
        if (callbackList.ContainsKey (pos))
            callbackList[pos] += callback;
        else
            callbackList.Add (pos, callback);
    }

    public void unsubscribeUpdate (Vector2 pos, System.Action<ItemStack> callback) {
        if (callbackList.ContainsKey (pos))
            callbackList[pos] -= callback;
    }

    public bool AddItem (ItemData item) {
        return AddItems (new ItemStack (item));
    }

    public bool AddItems (ItemStack stack) {
        Vector2 emptySlot = Vector2.one * -1;
        for (int y = 0; y < sizeInventory / sizeInventoryBar; y++) {
            for (int x = 0; x < sizeRowInventory && x + y * sizeInventoryBar < sizeInventory; x++) {
                Vector2 cursor = new Vector2 (x, y);
                if (inventoryData.ContainsKey (cursor)) {
                    if (inventoryData[cursor].data == stack.data) {
                        inventoryData[cursor].sizeStack += stack.sizeStack;
                        NotifyUpdate (cursor);
                        return true;
                    }
                } else if (emptySlot == Vector2.one * -1) {
                    emptySlot = cursor;
                }
            }
        }
        if (emptySlot != Vector2.one * -1) {
            inventoryData.Add (emptySlot, stack);
            NotifyUpdate (emptySlot);
            return true;
        }
        return false;
    }

    public void RemoveSlot (Vector2 pos) {
        inventoryData.Remove (pos);

        NotifyUpdate (pos);
    }

    public bool SetItemAt (Vector2 pos, ItemStack stack) {
        if (inventoryData.ContainsKey (pos)) {
            if (inventoryData[pos].data == stack.data) {
                inventoryData[pos].sizeStack += stack.sizeStack;
                NotifyUpdate (pos);
                return true;
            } else {
                return false;
            }
        }
        inventoryData.Add (pos, stack);
        NotifyUpdate (pos);
        return true;
    }

    void NotifyUpdate (Vector2 pos) {
        if (showInventory || pos.y == 0) {
            Debug.Log ("Notify Update at " + pos.ToString ());
            if (callbackList.ContainsKey (pos)) {
                if (inventoryData.ContainsKey (pos))
                    callbackList[pos] (inventoryData[pos]);
                else
                    callbackList[pos] (null);
            }
        }
    }

    void NotifyAll () {
        Vector2 cursor;
        for (int y = 0; y < sizeInventory / sizeInventoryBar; y++) {
            for (int x = 0; x < sizeRowInventory && x + y * sizeInventoryBar < sizeInventory; x++) {
                cursor = new Vector2 (x, y);
                NotifyUpdate (cursor);
            }
        }
    }

    public void initInventory () {
        if (inventoryData == null) {
            inventoryData = new Dictionary<Vector2, ItemStack> ();
        } else {
            inventoryData.Clear ();
        }
        foreach (var inventorySlot in initInventoryData) {
            inventoryData.Add (inventorySlot.pos, inventorySlot.itemStack);
            NotifyUpdate (inventorySlot.pos);
        }
    }

    private void Awake () {
        player = FindObjectOfType<RigidbodyFirstPersonController> ();
    }

    public void setUIVisible (bool visible) {
        showInventory = visible;
        background.SetActive (showInventory);
        if (showInventory) {
            NotifyAll ();
        }
        player.enabled = !showInventory;
        player.mouseLook.SetCursorLock (!showInventory);
    }

    private void Start () {
        initInventory ();
        setUIVisible (false);
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.I)) {
            setUIVisible (!showInventory);
        }
    }

    public ItemData getCurrentSlot () {
        Vector2 pos = new Vector2 (cursor.CurrentSelected, 0);
        if (inventoryData.ContainsKey (pos))
            return inventoryData[pos].data;
        return null;
    }

    public void RemoveItem (Vector2 pos, uint number = 1) {
        if (inventoryData.ContainsKey (pos)) {
            ItemStack stack = inventoryData[pos];
            if (stack.sizeStack <= number) {
                RemoveSlot (pos);
            } else {
                inventoryData[pos].sizeStack -= number;
                NotifyUpdate (pos);
            }
        }
    }

    public void RemoveCurrentItem (uint number = 1) {
        RemoveItem (new Vector2 (cursor.CurrentSelected, 0), number);
    }
}

[System.Serializable]
public class ItemStack {
    public ItemData data;
    public uint sizeStack;
    public ItemStack (ItemData _data) {
        data = _data;
        sizeStack = 1;
    }
}

[System.Serializable]
public struct InventorySlot {
    public Vector2 pos;
    public ItemStack itemStack;
}
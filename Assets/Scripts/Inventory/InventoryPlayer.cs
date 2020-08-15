using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class InventoryPlayer : IInventory {
    private static InventoryPlayer _instance;
    public static InventoryPlayer Instance {
        get {
            return _instance;
        }
    }
    public uint sizeInventory = 15;
    public const uint sizeInventoryBar = 5;
    public Dictionary<Vector2, System.Action<ItemStack>> callbackList = new Dictionary<Vector2, System.Action<ItemStack>> ();
    public SelectorCursorUI cursor = null;
    [SerializeField]
    private Text goldText = null;
    public int Gold {
        get {
            return inventoryData.gold;
        }
        set {
            inventoryData.gold = value;
            if (goldText != null)
                goldText.text = inventoryData.gold.ToString ();
        }
    }

    [SerializeField]
    const uint sizeRowInventory = 5;

    private static InventoryData inventoryData = null;
    public static InventoryData Inventory {
        get {
            return inventoryData;
        }
    }
    public InventoryData initData = null;
    private bool showInventory = false;
    public bool inventoryShown {
        get {
            return showInventory;
        }
    }

    [SerializeField]
    public GameObject background = null;
    [SerializeField]
    private List<GameObject> uiToHide = null;

    public enum SlotType {
        Plant,
        Tools,
        Other,
        Empty
    }

    [SerializeField]
    private Vector2 offsetBackground = new Vector2 (0, 350);
    private InventoryData.InventorySlot lastSlot;

    public override void subscribeUpdate (Vector2 pos, System.Action<ItemStack> callback) {
        if (callbackList.ContainsKey (pos))
            callbackList[pos] += callback;
        else
            callbackList.Add (pos, callback);

        if (showInventory || pos.y == 0) {
            foreach (var slot in inventoryData.inventoryContent) {
                if (slot.pos == pos) {
                    callback (slot.itemStack);
                    return;
                }
            }
            callback (null);
        }
    }

    public override void unsubscribeUpdate (Vector2 pos, System.Action<ItemStack> callback) {
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
                foreach (var slot in inventoryData.inventoryContent) {
                    if (slot.pos == cursor) {
                        if (slot.itemStack.data == stack.data) {
                            slot.itemStack.sizeStack += stack.sizeStack;
                            NotifyUpdate (slot);
                            return true;
                        }
                    }
                }
                if (emptySlot == Vector2.one * -1) {
                    emptySlot = cursor;
                }
            }
        }
        if (emptySlot != Vector2.one * -1) {
            InventoryData.InventorySlot slot = new InventoryData.InventorySlot (emptySlot, stack);
            inventoryData.inventoryContent.Add (slot);
            NotifyUpdate (slot);
            return true;
        }
        return false;
    }

    public override void RemoveSlot (Vector2 pos) {
        foreach (var slot in inventoryData.inventoryContent) {
            if (slot.pos == pos) {
                inventoryData.inventoryContent.Remove (slot);
                break;
            }
        }
        NotifyUpdate (new InventoryData.InventorySlot (pos, null));
    }

    public override bool SetItemAt (Vector2 pos, ItemStack stack) {
        foreach (var slot in inventoryData.inventoryContent) {
            if (slot.pos == pos) {
                if (slot.itemStack.data == stack.data) {
                    lastSlot = new InventoryData.InventorySlot (pos, stack);
                    slot.itemStack.sizeStack += stack.sizeStack;
                    NotifyUpdate (slot);
                    return true;
                } else {
                    return false;
                }
            }
        }
        InventoryData.InventorySlot _slot = new InventoryData.InventorySlot (pos, stack);
        lastSlot = _slot;
        inventoryData.inventoryContent.Add (_slot);
        NotifyUpdate (_slot);
        return true;
    }

    public void RewindLastItem () {
        InventoryData.InventorySlot rewindSlot = new InventoryData.InventorySlot (lastSlot.pos, null);
        foreach (var slot in inventoryData.inventoryContent) {
            if (slot.pos == lastSlot.pos) {
                Debug.LogFormat ("Rewind item: {0} x{1}", slot.itemStack.data.itemName, slot.itemStack.sizeStack);
                if (slot.itemStack.sizeStack == lastSlot.itemStack.sizeStack) {
                    inventoryData.inventoryContent.Remove (slot);
                } else {
                    slot.itemStack.sizeStack -= lastSlot.itemStack.sizeStack;
                    rewindSlot = slot;
                }
                break;
            }
        }
        NotifyUpdate (rewindSlot);
    }

    void NotifyUpdate (InventoryData.InventorySlot cursor) {
        if (showInventory || (cursor.pos.y == 0)) {
            if (callbackList.ContainsKey (cursor.pos)) {
                callbackList[cursor.pos] (cursor.itemStack);
            }
        }
    }

    public void NotifyAll () {
        List<Vector2> filledSlot = new List<Vector2> ();
        foreach (var slot in inventoryData.inventoryContent) {
            filledSlot.Add (slot.pos);
            NotifyUpdate (slot);
        }
        Vector2 cursor;
        for (int y = 0; y < sizeInventory / sizeInventoryBar; y++) {
            for (int x = 0; x < sizeRowInventory && x + y * sizeInventoryBar < sizeInventory; x++) {
                cursor = new Vector2 (x, y);
                if (!filledSlot.Contains (cursor))
                    NotifyUpdate (new InventoryData.InventorySlot (cursor, null));
            }
        }
    }

    public void initInventory () {
        if (inventoryData == null) {
            if (initData == null) {
                inventoryData = ScriptableObject.CreateInstance<InventoryData> ();
            } else {
                inventoryData = Instantiate (initData);
            }
        }
        foreach (var slot in inventoryData.inventoryContent) {
            NotifyUpdate (slot);
        }
    }

    private void Awake () {
        _instance = this;
        initInventory ();
        if (background != null)
            background.GetComponent<RectTransform> ().anchoredPosition = offsetBackground;
    }

    private void OnDestroy () {
        _instance = null;
    }

    public void setUIVisible (bool visible) {
        showInventory = visible;
        SlotUI slot;
        foreach (var ui in uiToHide) {
            slot = ui.GetComponent<SlotUI> ();
            if (slot) {
                slot.showSlot (showInventory);
            } else {
                ui.SetActive (showInventory);
            }
        }
        if (showInventory) {
            NotifyAll ();
            goldText.text = inventoryData.gold.ToString ();
        }
        RigidbodyFirstPersonController player = FindObjectOfType<RigidbodyFirstPersonController> ();
        if (player != null) {
            player.enabled = !showInventory;
            player.mouseLook.SetCursorLock (!showInventory);
        }
    }

    private void Start () {
        setUIVisible (false);
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.I)) {
            setUIVisible (!showInventory);
        }
    }

    public ItemData getCurrentSlot () {
        Vector2 pos = new Vector2 (cursor.CurrentSelected, 0);
        foreach (var slot in inventoryData.inventoryContent) {
            if (slot.pos == pos)
                return slot.itemStack.data;
        }
        return null;
    }

    public void RemoveItem (Vector2 pos, uint number = 1) {
        foreach (var slot in inventoryData.inventoryContent) {
            if (slot.pos == pos) {
                ItemStack stack = slot.itemStack;
                if (stack.sizeStack <= number) {
                    RemoveSlot (pos);
                } else {
                    slot.itemStack.sizeStack -= number;
                    NotifyUpdate (slot);
                }
                break;
            }
        }
    }

    public SlotType getCurrentSlotType () {
        ItemData item = getCurrentSlot ();
        if (item == null)
            return SlotType.Empty;
        switch (item.GetItemType) {
            case ItemData.ItemType.SeedType:
                return SlotType.Plant;
            case ItemData.ItemType.ToolType:
                return SlotType.Tools;
            default:
                return SlotType.Other;
        }
    }

    public void RemoveCurrentItem (uint number = 1) {
        RemoveItem (new Vector2 (cursor.CurrentSelected, 0), number);
    }

    private void OnValidate () {
        if (background != null)
            background.GetComponent<RectTransform> ().anchoredPosition = offsetBackground;
    }

    public static void setSavedInventory (SavableInventoryData savedData) {
        InventoryData data = InventoryData.CreateInstance<InventoryData> ();
        data.gold = savedData.gold;
        foreach (var _slot in savedData.inventoryContent) {
            ItemData item = ItemFactory.Instance.MakeItem (_slot.itemStack.data);
            ItemStack stack = new ItemStack (item, _slot.itemStack.sizeStack);
            InventoryData.InventorySlot slot = new InventoryData.InventorySlot (_slot.pos, stack);
            data.inventoryContent.Add (slot);
        }
        inventoryData = data;
        if (_instance != null) {
            _instance.NotifyAll ();
        }
    }
}
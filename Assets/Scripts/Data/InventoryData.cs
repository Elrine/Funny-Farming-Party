using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Inventory/InventoryData")]
public class InventoryData : ScriptableObject {
    public List<InventorySlot> inventoryContent = new List<InventorySlot>();
    public int gold;
    [System.Serializable]
    public class InventorySlot {
        public Vector2 pos;
        public ItemStack itemStack;
        public InventorySlot (Vector2 _pos, ItemStack _itemStack) {
            pos = _pos;
            itemStack = _itemStack;
        }

        public SavableInventorySlot ToSavableData() {
            SavableInventorySlot toSave = new SavableInventorySlot();
            toSave.pos = pos;
            toSave.itemStack = itemStack.ToSavableData();
            return toSave;
        }
    }

    public SavableInventoryData ToSavableData() {
        SavableInventoryData toSave = new SavableInventoryData();
        foreach (var slot in inventoryContent)
        {
            toSave.inventoryContent.Add(slot.ToSavableData());
        }
        toSave.gold = gold;
        return toSave;
    }
}

[System.Serializable]
public class SavableInventoryData {
    public List<SavableInventorySlot> inventoryContent = new List<SavableInventorySlot>();
    public int gold;
}

[System.Serializable]
public class SavableInventorySlot {
    public Vector2 pos;
    public SavableItemStack itemStack;
}

[System.Serializable]
public class ItemStack {
    public ItemData data;
    public int sizeStack;
    public ItemStack (ItemData _data, int _sizeStack = 1) {
        data = _data;
        sizeStack = _sizeStack;
    }

    public SavableItemStack ToSavableData () {
        SavableItemStack toSave = new SavableItemStack ();
        toSave.data = data.ToSavableData ();
        toSave.sizeStack = sizeStack;
        return toSave;
    }
}

[System.Serializable]
public class SavableItemStack {
    public SavableItemData data;
    public int sizeStack;
}
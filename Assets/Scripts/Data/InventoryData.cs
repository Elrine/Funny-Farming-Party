using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Inventory/InventoryData")]
public class InventoryData : ScriptableObject {
    public List<InventorySlot> inventoryContent;
    public int gold;
    [System.Serializable]
    public class InventorySlot {
        public Vector2 pos;
        public ItemStack itemStack;
        public InventorySlot (Vector2 _pos, ItemStack _itemStack) {
            pos = _pos;
            itemStack = _itemStack;
        }
    }
}
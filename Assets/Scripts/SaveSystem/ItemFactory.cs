using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour {
    [System.Serializable]
    public class ItemTypeRow {
        public ItemData.ItemType type;
        public ItemData[] ModelList;
    }
    public List<ItemTypeRow> itemType = new List<ItemTypeRow> ();
    private static ItemFactory _instance = null;
    public static ItemFactory Instance {
        get {
            return _instance;
        }
    }

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (gameObject);
        }
    }

    public ItemData MakeItem (SavableItemData itemData) {
        if (itemType.Find ((item) => item.type == itemData.itemType) != null) {
            foreach (var item in itemType.Find ((item) => item.type == itemData.itemType).ModelList) {
                if (item.itemName == itemData.itemName) {
                    return item;
                }
            }
        }
        return null;
    }
}
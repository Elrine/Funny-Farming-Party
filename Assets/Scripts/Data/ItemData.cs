using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomData/Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [SerializeField]
    [TextArea]
    protected string itemDescription;
    public GameObject itemInWorld;
    public Texture2D itemInHUD;
    public virtual string ItemDescription {
        get {
            return itemDescription;
        }
    }
    public enum ItemType {
        SeedType,
        ToolType,
        SpellType,
        RessourceType,
        OtherType
    }
    public virtual ItemType GetItemType {
        get{
            return ItemType.OtherType;
        }
    }
    
    public Attribute attribute = Attribute.Neutral;
    public int attributeLevel;
    public int goldValue;

    public SavableItemData ToSavableData() {
        SavableItemData toSave = new SavableItemData();
        toSave.itemName = itemName;
        toSave.itemType = GetItemType;
        return toSave;
    }
}

[System.Serializable]
public class SavableItemData {
    public string itemName;
    public ItemData.ItemType itemType;
}

public enum Attribute {
        Earth,
        Water,
        Air,
        Fire,
        Order,
        Chaos,
        Neutral
    }
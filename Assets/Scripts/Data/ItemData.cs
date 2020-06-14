using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
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
        RessourceType,
        OtherType
    }
    public virtual ItemType GetItemType {
        get{
            return ItemType.OtherType;
        }
    }
    public int goldValue;
}

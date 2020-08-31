using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomData/Receipe/ReceipeData")]
public class ReceipeData : ScriptableObject
{
    public const int MAX_ITEM = 5; 
    public enum ReceipeType {
        item,
        attribute
    }
    public ReceipeType type;
    [SerializeField]
    public ItemData[] itemSource = new ItemData[MAX_ITEM];
    [System.Serializable]
    public class AttributeData {
        public Attribute type;
        public int needed;
    }
    public AttributeData[] attributes = new AttributeData[MAX_ITEM];
    public ItemData result;
}

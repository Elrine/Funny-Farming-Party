using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Inventory/StoreData")]
public class StoreData : ScriptableObject {
    public List<ItemData> sellItem;
}
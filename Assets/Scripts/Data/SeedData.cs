using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class SeedData : ItemData {
    public override ItemType GetItemType {
        get {
            return ItemType.SeedType;
        }
    }
    public override string ItemDescription {
        get {
            if (seedOf != null) {
                return "Seed of: " + seedOf.ressourceName + "\nPlant description:\n" + seedOf.description + "\nSeed description:\n" + itemDescription;
            } else {
                return itemDescription;
            }
        }
    }
    public PlantData seedOf;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class RessourceData : ScriptableObject {
    public string ressourceName;
    [TextArea]
    public string description;
    public Drop[] dropItem;
    public GameObject prefab;
    public float sizeRessource = 1;
    
    public enum RessourceType {
        PlantType,
        OtherType
    }
    public virtual RessourceType GetRessourceType {
        get{
            return RessourceType.OtherType;
        }
    }

    public SavableRessourceData ToSavableData() {
        SavableRessourceData toSave = new SavableRessourceData();
        toSave.type = GetRessourceType;
        toSave.ressourceName = ressourceName;
        return toSave;
    }

    [System.Serializable]
    public class Drop {
        public ItemData item;
        [Range (0, 1)]
        public float dropRate;
        public int minDrop;
        public int maxDrop;
        public AnimationCurve numberDrop;
        public bool dropWhenMaturate = false;

        public class SortByDropRate : IComparer<Drop> {
            public int Compare (Drop x, Drop y) {
                return x.dropRate.CompareTo (y.dropRate);
            }
        }
    }
}

[System.Serializable]
public class SavableRessourceData {
    public string ressourceName;
    public RessourceData.RessourceType type;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RessourceData : UpdatebleData
{
    public string ressourceName;
    [TextArea]
    public string description;
    public Drop[] dropItem;
    public GameObject prefab;

    [System.Serializable]
    public class Drop {
        public string name;
        [Range(0,1)]
        public float dropRate;
        public int minDrop;
        public int maxDrop;
        public GameObject item;
    }
}

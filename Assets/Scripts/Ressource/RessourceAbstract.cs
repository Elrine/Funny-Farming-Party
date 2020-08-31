using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RessourceAbstact : MonoBehaviour {
    [SerializeField]
    public RessourceData ressourceType = null;
    protected bool isMature = true;
    public Vector2 posInGrid = Vector2.zero;

    public void Harvest () {
        RessourceData.Drop[] dropList = ressourceType.dropItem;
        Array.Sort (dropList, new RessourceData.Drop.SortByDropRate ());
        float percent = UnityEngine.Random.Range (0, 10000) / 10000f;
        foreach (var drop in dropList) {
            if (percent <= drop.dropRate && (!drop.dropWhenMaturate || isMature)) {
                int numberOfDrop;
                if (!isMature)
                    numberOfDrop = Mathf.RoundToInt (drop.numberDrop.Evaluate (percent * drop.dropRate) * (drop.maxDrop - drop.minDrop)) + drop.minDrop;
                else
                    numberOfDrop = drop.minDrop;
                for (int i = 0; i < numberOfDrop; i++) {
                    GameObject itemInWrold = GameObject.Instantiate (drop.item.itemInWorld, transform.position + Vector3.up * .5f, Quaternion.identity);
                    Rigidbody rigidbody = itemInWrold.GetComponent<Rigidbody> ();
                    Vector2 randPos = UnityEngine.Random.insideUnitCircle;
                    rigidbody.velocity = new Vector3 (randPos.x / 2f, 1, randPos.y / 2f) * 5;
                }
            }
        }
        OnHarvesting ();
    }

    protected abstract void OnHarvesting ();

    public virtual SavableRessourceAbstact ToSavableData() {
        SavableRessourceAbstact toSave = new SavableRessourceAbstact();
        toSave.data = ressourceType.ToSavableData();
        return toSave;
    }
}

[System.Serializable]
public class SavableRessourceAbstact {
    public SavableRessourceData data;
}
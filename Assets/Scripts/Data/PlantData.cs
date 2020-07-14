using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlantData : RessourceData
{
    public float daysToGrow;
    public override RessourceType GetRessourceType {
        get {
            return RessourceType.PlantType;
        }
    }
}

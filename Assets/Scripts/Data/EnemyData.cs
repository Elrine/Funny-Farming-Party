using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "CustomData/Ressource/EnemyData")]
public class EnemyData : RessourceData
{
    public override RessourceType GetRessourceType {
        get{
            return RessourceType.EnemyType;
        }
    }

    public int maxHealth;
    public Attribute attribute;
    public int attributeLevel;
}

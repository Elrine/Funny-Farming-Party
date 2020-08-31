using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Item/SpellData")]
public class SpellData : ItemData
{
    public override ItemType GetItemType {
        get {
            return ItemType.SpellType;
        }
    }

    public enum TargetType
    {
        Self,
        Ray,
        Projectile
    }
    public TargetType target;
    public enum ActionType {
        Hurt,
        Grow,
        Heal
    }
    public ActionType action;
    public bool strengthenEffect;
    public GameObject projectile = null;
}

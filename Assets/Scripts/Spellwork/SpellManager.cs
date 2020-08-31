using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellManager {
    private static SpellData currentSpell = null;
    private static int _currentStrength = 0;
    public static int CurrentStrength {
        get {
            return _currentStrength;
        }
    }
    public static bool isCasting = false;
    public static void StartCasting (SpellData data) {
        currentSpell = data;
        _currentStrength = 1;
        isCasting = true;
        if (currentSpell.target == SpellData.TargetType.Ray) {
            BeamControl.instance.Setup (data);
        }
    }

    public static void AddingStrength () {
        if (isCasting) {
            _currentStrength++;
            if (BeamControl.instance.isShown)
                Ray ();
        }
    }

    public static void StopCasting () {
        isCasting = false;
        if (currentSpell) {
            switch (currentSpell.target) {
                case SpellData.TargetType.Projectile:
                    Projectile ();
                    break;
                case SpellData.TargetType.Self:
                    Self ();
                    break;
                case SpellData.TargetType.Ray:
                    BeamControl.instance.Stop ();
                    break;
                default:
                    break;
            }
            currentSpell = null;
        }
    }

    public static void Projectile () {
        GameObject projectileSpawn = GameObject.FindGameObjectWithTag ("ProjectileSpawn");
        GameObject projectile = GameObject.Instantiate (currentSpell.projectile, projectileSpawn.transform);
        projectile.GetComponent<Projectile> ().Setup (Camera.main.transform.forward, currentSpell, _currentStrength);
    }
    public static void Self () {
        Effect (GameObject.FindObjectOfType<PlayerHealth> ().gameObject);
    }
    public static void Ray () {
        RaycastHit[] hits = Physics.RaycastAll (GameObject.FindGameObjectWithTag ("ProjectileSpawn").transform.position, Camera.main.transform.forward, 20);
        foreach (var hit in hits) {
            if (!hit.collider.gameObject.CompareTag ("Chunk") && !hit.collider.gameObject.CompareTag ("Projectile")) {
                Effect (hit.collider.gameObject);
                break;
            }
        }
    }

    public static void Effect (GameObject target, SpellData _spell = null, int _strength = -1) {
        SpellData spell = _spell == null ? currentSpell : _spell;
        int strength = _strength == -1 ? _currentStrength : _strength;
        switch (spell.action) {
            case SpellData.ActionType.Grow:
                Plant plant = target.GetComponent<Plant> ();
                if (plant != null) {
                    plant.AccelerateGrow (.01f * spell.attributeLevel * (spell.strengthenEffect ? _currentStrength : 1));
                }
                break;
            case SpellData.ActionType.Hurt:
                IDamagable damagable = target.GetComponent<IDamagable> ();
                damagable.hit (spell.attributeLevel + spell.attributeLevel * (spell.strengthenEffect ? _currentStrength / 10 : 0), spell.attribute);
                break;
            case SpellData.ActionType.Heal:
                IDamagable toHeal = target.GetComponent<IDamagable> ();
                toHeal.heal (spell.attributeLevel + spell.attributeLevel * (spell.strengthenEffect ? _currentStrength / 10 : 0));
                break;
            default:
                break;
        }
    }
}
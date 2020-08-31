using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Projectile : MonoBehaviour {
    private SpellData _data = null;
    private int _strength = -1;
    [SerializeField]
    private int speed = 100;
    [SerializeField]
    private int delayToDie = 10;
    private float deltaTime = -1;
    [SerializeField]
    private Color earthColor;
    [SerializeField]
    private Color airColor;
    [SerializeField]
    private Color waterColor;
    [SerializeField]
    private Color fireColor;
    [SerializeField]
    private Color orderColor;
    [SerializeField]
    private Color chaosColor;

    public void Setup (Vector3 dir, SpellData data, int strength) {
        _data = data;
        _strength = strength;
        Color currentColor = new Color ();
        switch (data.attribute) {
            case Attribute.Earth:
                currentColor = earthColor;
                break;
            case Attribute.Air:
                currentColor = airColor;
                break;
            case Attribute.Water:
                currentColor = waterColor;
                break;
            case Attribute.Fire:
                currentColor = fireColor;
                break;
            case Attribute.Order:
                currentColor = orderColor;
                break;
            case Attribute.Chaos:
                currentColor = chaosColor;
                break;
        }
        gameObject.GetComponent<MeshRenderer> ().material.color = currentColor;
        gameObject.GetComponent<Rigidbody> ().AddForce (dir * speed, ForceMode.Impulse);
        deltaTime = 0;
    }

    private void Update () {
        if (deltaTime >= 0)
            deltaTime += Time.deltaTime;
        if (deltaTime > delayToDie) {
            Destroy (gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        SpellManager.Effect(other.gameObject, _data, _strength);
        Destroy(gameObject);
    }
}
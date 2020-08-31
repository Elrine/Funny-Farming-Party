using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : RessourceAbstact, IDamagable {
    public int currentHealth = 0;
    [SerializeField]
    private int updateDelay = 2;
    private float timeDelay = 0;

    public void heal (int value) {
        EnemyData _data = ressourceType as EnemyData;
        currentHealth += value;
        if (currentHealth > _data.maxHealth)
            currentHealth = _data.maxHealth;
    }

    public void hit (int damage, Attribute attackType) {
        EnemyData _data = ressourceType as EnemyData;
        float multiplier = 1;
        if (attackType == _data.attribute) {
            multiplier -= 0.5f;
        }
        if ((attackType == Attribute.Earth && _data.attribute == Attribute.Fire) ||
            (attackType == Attribute.Air && _data.attribute == Attribute.Water) ||
            (attackType == Attribute.Water && _data.attribute == Attribute.Earth) ||
            (attackType == Attribute.Fire && _data.attribute == Attribute.Air) ||
            (attackType == Attribute.Order && _data.attribute == Attribute.Chaos) ||
            (attackType == Attribute.Chaos && _data.attribute == Attribute.Order))
            multiplier += 1;
        currentHealth -= Mathf.RoundToInt (damage * multiplier);
        if (currentHealth <= 0) {
            Harvest ();
        }
    }

    protected override void OnHarvesting () {
        GameObject.Destroy (gameObject);
    }

    private void Update () {
        timeDelay += Time.deltaTime;
        if (timeDelay > updateDelay) {
            gameObject.GetComponent<NavMeshAgent> ().SetDestination (GameObject.FindGameObjectWithTag ("Player").transform.position);
            timeDelay = 0;
        }
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.CompareTag ("Player")) {
            EnemyData _data = ressourceType as EnemyData;
            other.GetComponent<PlayerHealth> ().hit (_data.attributeLevel, _data.attribute);
        }
    }

    private void OnTriggerStay (Collider other) {
        if (other.gameObject.CompareTag ("Player") && timeDelay / 2 >= 1) {
            EnemyData _data = ressourceType as EnemyData;
            other.GetComponent<PlayerHealth> ().hit (_data.attributeLevel, _data.attribute);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable {
    public int currentHealth = 0;
    [SerializeField]
    private int maxHealth;
    public int MaxHealth {
        get {
            return maxHealth;
        }
    }
    private float deltaTime = 0;
    
    public void heal (int value) {
        currentHealth += value;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void hit (int damage, Attribute attackType) {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Death ();
    }

    void Death () {
        DeathMenu.Instance.SetVisible(true);
    }

    // Start is called before the first frame update
    void Start () {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update () {
        if (Loader.getCurrentScene() == Loader.Scene.OuterWorld && Vector3.Distance(transform.position, SaveManager.Instance.env.portalPosition) > SaveManager.Instance.env.forceFieldSize) {
            deltaTime = deltaTime == -1 ? 0 : deltaTime + Time.deltaTime;
            if (deltaTime > 1) {
                hit(5,Attribute.Neutral);
                deltaTime = 0;
            }
        }
    }
}
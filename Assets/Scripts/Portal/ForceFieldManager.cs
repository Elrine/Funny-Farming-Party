using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ForceFieldManager : MonoBehaviour
{
    [SerializeField]
    VisualEffect effect;
    [SerializeField]
    float radius = 20;

    public float Radius {
        get
        {
            return radius;
        }
        set {
            radius = value;
            SaveManager.Instance.env.forceFieldSize = radius;
            effect.SetFloat("Radius", radius);
        }
    }

    private static SaveManager GetInstance()
    {
        return SaveManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        effect.enabled = Loader.getCurrentScene() == Loader.Scene.OuterWorld;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateForceField() {
        Radius += 10;
    }

    private void OnValidate() {
        if (effect != null) {
            effect.SetFloat("Radius", radius);
        }
    }
}

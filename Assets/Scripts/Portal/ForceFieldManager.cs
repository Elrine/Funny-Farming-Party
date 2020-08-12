using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ForceFieldManager : MonoBehaviour
{
    [SerializeField]
    GameObject forceField = null;
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
            forceField.transform.localScale  = Vector3.one * radius * 2;
        }
    }

    private static SaveManager GetInstance()
    {
        return SaveManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        forceField.SetActive(Loader.getCurrentScene() == Loader.Scene.OuterWorld);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateForceField() {
        Radius += 10;
    }

    private void OnValidate() {
        if (forceField != null) {
            forceField.transform.localScale  = Vector3.one * radius * 2;
        }
    }
}

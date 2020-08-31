using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamControl : MonoBehaviour
{
    public static BeamControl instance = null;
    [SerializeField]
    private MeshRenderer rendererShader = null;
    [SerializeField]
    private MeshRenderer rendererCylinder = null;
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
    private Color32 chaosColor;
    private SpellData _data = null;

    public bool isShown = false;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        rendererCylinder.enabled = isShown;
        rendererShader.enabled = isShown;
    }

    public void Setup (SpellData data) {
        _data = data;
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
        rendererShader.material.SetColor("_Color",currentColor);
        rendererCylinder.material.color = currentColor;
        isShown = true;
        rendererCylinder.enabled = isShown;
        rendererShader.enabled = isShown;
    }

    public void Stop() {
        isShown = false;
        rendererCylinder.enabled = isShown;
        rendererShader.enabled = isShown;
    }
}

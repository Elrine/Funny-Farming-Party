using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironementData
{
    public int seed = 0;
    public Vector3 portalPosition = Vector3.negativeInfinity;
    public float forceFieldSize = 20;
    public Vector3 playerPosition = Vector3.negativeInfinity;
    public Quaternion playerRotation = Quaternion.identity;
    public float currentTime;
    public Loader.Scene playerScene = Loader.Scene.OuterWorld;
    public bool isCreated = false;
}

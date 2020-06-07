using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    private float _minutePerDay = 24f;
    public float minutePerDay {
        get {
            return _minutePerDay;
        }
    }
    [Range(0,1)]
    public float currentTime;
    [SerializeField]
    private Material skyMaterial;
    [SerializeField]
    private Gradient gradientSky;
    [SerializeField]
    private Gradient gradientGround;
    private float timeScale;

    // Update is called once per frame
    void Update()
    {
        UpdateTimeScale();
        UpdateCurrentTime();
        UpdateLight();
    }

    void UpdateCurrentTime () {
        currentTime += Time.deltaTime * timeScale / 86400;
        if (currentTime >= 1) {
            currentTime -= 1;
        }
    }

    void UpdateTimeScale() {
        timeScale = 24 / (minutePerDay / 60);
    }

    void UpdateLight () {
        transform.localRotation = Quaternion.Euler((-currentTime * 360)-180, 0, 0);
    }

    private void OnValidate() {
        UpdateTimeScale();
        UpdateLight();
    }
}

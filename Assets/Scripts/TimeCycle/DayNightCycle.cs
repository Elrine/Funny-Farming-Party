using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {
    [SerializeField]
    private float _minutePerDay = 24f;
    public float minutePerDay {
        get {
            return _minutePerDay;
        }
    }

    [Range (0, 1)]
    public float currentTime;
    [SerializeField]
    private Material skyMaterial = null;
    [SerializeField]
    private Gradient gradientSky = new Gradient();
    [SerializeField]
    private Gradient gradientGround = new Gradient();
    private float _timeScale;
    [SerializeField]
    private AnimationCurve lightCurve = new AnimationCurve();

    // Update is called once per frame
    void Update () {
        UpdateTimeScale ();
        UpdateCurrentTime ();
        UpdateLight ();
        updateMaterial ();
    }

    void UpdateCurrentTime () {
        currentTime += Time.deltaTime * _timeScale / 86400;
        if (currentTime >= 1) {
            currentTime -= 1;
        }
    }

    void UpdateTimeScale () {
        _timeScale = 24 / (minutePerDay / 60);
    }

    void UpdateLight () {
        transform.localRotation = Quaternion.Euler ((-currentTime * 360) - 180, 0, 0);
    }

    void updateMaterial () {
        if (skyMaterial != null) {
            skyMaterial.SetColor ("_SkyTint",  gradientSky.Evaluate (currentTime));
            skyMaterial.SetFloat ("_SunStrength", lightCurve.Evaluate (currentTime));
        }
    }

    public float getDeltaTime() {
        return Time.deltaTime * _timeScale / 86400;
    }

    private void OnValidate () {
        UpdateTimeScale ();
        UpdateLight ();
        updateMaterial ();
    }
}
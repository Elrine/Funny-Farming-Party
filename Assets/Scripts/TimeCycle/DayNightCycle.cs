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
    private Gradient gradientSky = new Gradient ();
    [SerializeField]
    private Gradient gradientGround = new Gradient ();
    private float _timeScale;
    [SerializeField]
    private AnimationCurve lightCurve = new AnimationCurve ();
    private Light _light;
    private static DayNightCycle _instance = null;
    public static DayNightCycle Instance {
        get {
            return _instance;
        }
    }

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad (gameObject);
            setLight ();
        } else {
            Destroy (gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
        UpdateMinutePerDay ();
        UpdateTimeScale ();
        UpdateCurrentTime ();
        UpdateLight ();
        updateMaterial ();
    }

    private void UpdateMinutePerDay () {
        if (Input.GetKey (KeyCode.BackQuote)) {
            if (Input.GetKeyDown (KeyCode.Keypad0))
                _minutePerDay = 24;
            if (Input.GetKeyDown (KeyCode.Keypad1))
                _minutePerDay = 12;
            if (Input.GetKeyDown (KeyCode.Keypad2))
                _minutePerDay = 5;
            if (Input.GetKeyDown (KeyCode.Keypad3))
                _minutePerDay = 1;
            if (Input.GetKeyDown (KeyCode.Keypad4))
                _minutePerDay = .1f;
        }
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
            skyMaterial.SetColor ("_SkyTint", gradientSky.Evaluate (currentTime));
            skyMaterial.SetFloat ("_SunStrength", lightCurve.Evaluate (currentTime));
            _light.intensity = lightCurve.Evaluate (currentTime);
        }
    }

    public float getDeltaTime () {
        return Time.deltaTime * _timeScale / 86400;
    }

    public float getDeltaTime (float prevTime) {
        return (Time.time - prevTime) * _timeScale / 86400;
    }

    private void OnValidate () {
        setLight ();
        UpdateTimeScale ();
        UpdateLight ();
        updateMaterial ();
    }

    private void setLight () {
        _light = GetComponentInChildren<Light> ();
    }
}
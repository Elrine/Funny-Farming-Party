using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour {
    public GameObject canvas;
    private bool isShown = false;
    private static DeathMenu _instance = null;
    public static DeathMenu Instance {
        get {
            return _instance;
        }
    }
    // Start is called before the first frame update
    public void Awake () {
        if (_instance != null) {
            Destroy (gameObject);
        } else {
            _instance = this;
            SetVisible(isShown);
        }
    }

    public void SetVisible(bool isVisible) {
        canvas.SetActive(isVisible);
        isShown = isVisible;
    }

    public void ToMenu() {
        SaveManager.Instance.env = null;
        Loader.LoadScrene(Loader.Scene.Menu);
    }

    public void Quit() {
        Application.Quit();
    }
}
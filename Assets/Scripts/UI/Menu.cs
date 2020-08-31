using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject canvas;
    private static Menu _instance = null;
    public bool isShown = false;
    public static Menu Instance {
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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        SetVisible(!isShown);
    }

    public void SetVisible(bool isVisible) {
        canvas.SetActive(isVisible);
        isShown = isVisible;
        RigidbodyFirstPersonController player = FindObjectOfType<RigidbodyFirstPersonController> ();
        if (player != null) {
            player.enabled = !isVisible;
            player.mouseLook.SetCursorLock (!isVisible);
        }
    }

    public void ToMenu() {
        SaveManager.Instance.env = null;
        Loader.LoadScrene(Loader.Scene.Menu);
        SetVisible(false);
    }

    public void Quit() {
        Application.Quit();
    }

    public void Save() {
        SaveManager.Instance.SaveWorld();
    }
}

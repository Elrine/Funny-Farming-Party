using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceipeFactory : MonoBehaviour
{
    [SerializeField]
    List<ReceipeData> listReceipe = new List<ReceipeData>();
    private static ReceipeFactory _instance = null;
    public static ReceipeFactory Instance {
        get {
            return _instance;
        }
    }

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (this);
        }
    }
}

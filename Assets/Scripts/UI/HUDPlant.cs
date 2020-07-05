using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPlant : MonoBehaviour
{
    public Plant target = null;
    [SerializeField]
    private Text nameText = null;
    [SerializeField]
    private SimpleHealthBar progressBar = null;
    [SerializeField]
    private Text descriptionText = null;
    [SerializeField]
    private Text growValueText = null;
    private static HUDPlant _instance = null;
    public static HUDPlant Instance {
        get {
            return _instance;
        }
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        initFromTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) {
            initFromTarget();
            progressBar.UpdateBar(target.CurrentGrowth, 1);
        }
    }

    private void OnValidate() {
        initFromTarget();
    }

    void initFromTarget() {
        if (target != null) {
            nameText.text = target.PlantType.seedOf.ressourceName;
            descriptionText.text = string.Format( "Description: {0}", target.PlantType.seedOf.description);
            growValueText.text = string.Format("Day require to grow :\n{0} day(s)", target.PlantType.seedOf.daysToGrow);
        }
    }
}

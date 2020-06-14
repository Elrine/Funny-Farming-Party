using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPlant : MonoBehaviour
{
    public Plant target;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private SimpleHealthBar progressBar;
    [SerializeField]
    private Text descriptionText;
    [SerializeField]
    private Text growValueText;

    // Start is called before the first frame update
    void Start()
    {
        initFromTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) {
            progressBar.UpdateBar(target.CurrentGrowth, 1);
        }
    }

    private void OnValidate() {
        initFromTarget();
    }

    void initFromTarget() {
        if (target != null) {
            nameText.text = target.PlantType.ressourceName;
            descriptionText.text = string.Format( "Description: {0}", target.PlantType.description);
            growValueText.text = string.Format("Day require to grow :\n{0} day(s)", target.PlantType.daysToGrow);
        }
    }
}

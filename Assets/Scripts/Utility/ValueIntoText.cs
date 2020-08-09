using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ValueIntoText : MonoBehaviour
{
    public void changeValue(float value) {
        Text text = gameObject.GetComponent<Text>();
        text.text = Mathf.FloorToInt(value).ToString();
    }
}

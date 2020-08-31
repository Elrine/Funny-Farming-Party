using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
    private static Tooltip instance = null;

    private Text itemText;
    private RectTransform backgroundRect;
    [SerializeField]
    private float textPadding = 4f;

    private void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad (gameObject);
            itemText = transform.Find ("itemText").GetComponent<Text> ();
            itemText.GetComponent<RectTransform>().localPosition = Vector2.one * textPadding;
            backgroundRect = transform.Find ("background").GetComponent<RectTransform> ();
            HideTooltip();
        } else
            Destroy (gameObject);
    }

    private void Update () {
        Rect parentRect = transform.parent.GetComponent<RectTransform>().rect;
        Vector2 localPoint = new Vector2(Input.mousePosition.x - parentRect.width / 2, Input.mousePosition.y - parentRect.height / 2);
        transform.localPosition = localPoint;
    }

    private void ShowTooltip (ItemStack item) {
        gameObject.SetActive (true);
        transform.SetAsLastSibling();

        itemText.text = string.Format ("{0}\nValue : {1}", item.data.itemName, item.data.goldValue * item.sizeStack);
        Vector2 backgroundSize = new Vector2 (itemText.preferredWidth + textPadding * 2, itemText.preferredHeight + textPadding * 2);
        backgroundRect.sizeDelta = backgroundSize;
    }

    private void HideTooltip () {
        gameObject.SetActive (false);
    }

    public static void ShowTooltip_static (ItemStack item) {
        if (instance != null)
            instance.ShowTooltip (item);
    }

    public static void HideTooltip_static () {
        if (instance != null)
            instance.HideTooltip ();
    }
}
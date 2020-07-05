using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
    public ItemStack itemStack = null;
    public Canvas canvas = null;
    public SlotUI fromSlot = null;
    private Text stackNumberUI;
    private RawImage itemImage;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool inSlot = false;

    public void OnBeginDrag (PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .6f;
    }

    public void OnDrag (PointerEventData eventData) {
        if (canvas != null)
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag (PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        resetToSlot ();
    }

    public void OnDrop (PointerEventData eventData) {
        fromSlot.OnDrop (eventData);
    }

    // Start is called before the first frame update
    private void Awake () {
        canvasGroup = GetComponent<CanvasGroup> ();
        stackNumberUI = GetComponentInChildren<Text> ();
        itemImage = GetComponent<RawImage> ();
        rectTransform = GetComponent<RectTransform> ();
    }

    private void Start () {
        if (itemStack != null) {
            itemImage.texture = itemStack.data.itemInHUD;
            stackNumberUI.text = itemStack.sizeStack.ToString ();
        }
        if (fromSlot != null) {
            rectTransform.anchoredPosition = fromSlot.GetComponent<RectTransform> ().anchoredPosition;
            inSlot = true;
        }
    }

    // Update is called once per frame
    public void UpdateStack (ItemStack stack) {
        itemStack = stack;
        itemImage.texture = itemStack.data.itemInHUD;
        stackNumberUI.text = itemStack.sizeStack.ToString ();
    }

    private void Update () {
        if (!inSlot && fromSlot != null) {
            rectTransform.anchoredPosition = fromSlot.GetComponent<RectTransform> ().anchoredPosition;
            inSlot = true;
        }
    }

    public void resetToSlot () {
        rectTransform.anchoredPosition = fromSlot.GetComponent<RectTransform> ().anchoredPosition;
    }

    public void setSlot (SlotUI newSlot) {
        if (fromSlot != null)
            fromSlot.removeCurrentItem ();
        if (newSlot != null) {
            rectTransform.SetParent (newSlot.GetComponent<RectTransform> ().parent, false);
        }
        fromSlot = newSlot;
    }

    public void removeItem() {
        Debug.LogFormat("Destroy {0}", fromSlot.position);
        fromSlot.removeCurrentItem();
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IDropHandler {
    public Vector2 position = Vector2.zero;
    public Canvas canvas;
    public GameObject itemPrefab;
    public ItemUI currentItem = null;
    [SerializeField]
    public IInventory currentInventory = null;

    private void Start () {
        if (currentInventory == null) {
            currentInventory = InventoryPlayer.Instance;
        }
        currentInventory.subscribeUpdate (position, inventoryUpdated);
    }

    private void OnDestroy () {
        currentInventory.unsubscribeUpdate (position, inventoryUpdated);
    }

    void inventoryUpdated (ItemStack stack) {
        if (stack == null) {
            if (currentItem != null) {
                currentItem.removeItem ();
            }
        } else if (currentItem == null) {
            GameObject itemUi = GameObject.Instantiate (itemPrefab, transform.parent);
            currentItem = itemUi.GetComponent<ItemUI> ();
            currentItem.canvas = canvas;
            currentItem.UpdateStack (stack);
            currentItem.setSlot (this);
        } else {
            currentItem.UpdateStack (stack);
        }
    }

    public void removeCurrentItem () {
        currentItem = null;
        currentInventory.RemoveSlot (position);
    }

    public void OnDrop (PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            eventData.pointerDrag.GetComponent<RectTransform> ().anchoredPosition = GetComponent<RectTransform> ().anchoredPosition;
            ItemUI newItem = eventData.pointerDrag.GetComponent<ItemUI> ();
            if (newItem != null) {
                if (currentItem == null) {
                    currentItem = newItem;
                    newItem.setSlot (this);
                    currentInventory.SetItemAt (position, newItem.itemStack);
                } else if (currentInventory.SetItemAt (position, newItem.itemStack)) {
                    newItem.removeItem ();
                    Destroy (newItem.gameObject);
                } else {
                    newItem.resetToSlot ();
                }
            }
        }
    }

    public void showSlot (bool showSlot) {
        if (currentItem)
            currentItem.gameObject.SetActive (showSlot);
        gameObject.SetActive (showSlot);
    }
}
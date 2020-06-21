using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IDropHandler {
    public Vector2 position = Vector2.zero;
    public InventoryPlayer inventory;
    public Canvas canvas;
    public GameObject itemPrefab;
    public ItemUI currentItem = null;

    private void Awake () {
        inventory = FindObjectOfType<InventoryPlayer> ();
        inventory.subscribeUpdate (position, inventoryUpdated);
    }

    void inventoryUpdated (ItemStack stack) {
        if (stack == null) {
            if (currentItem != null) {
                GameObject.Destroy (currentItem.gameObject);
                currentItem = null;
            }
        } else if (currentItem == null) {
            GameObject itemUi = GameObject.Instantiate (itemPrefab, transform.parent);
            currentItem = itemUi.GetComponent<ItemUI> ();
            currentItem.canvas = canvas;
            currentItem.UpdateStack (stack);
            currentItem.setSlot(this);
        } else {
            currentItem.UpdateStack (stack);
        }
    }

    public void removeCurrentItem() {
        currentItem = null;
        inventory.RemoveSlot(position);
    }

    public void OnDrop (PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            eventData.pointerDrag.GetComponent<RectTransform> ().anchoredPosition = GetComponent<RectTransform> ().anchoredPosition;
            ItemUI newItem = eventData.pointerDrag.GetComponent<ItemUI> ();
            if (newItem != null) {
                if (currentItem == null) {
                    currentItem = newItem;
                    newItem.setSlot(this);
                    inventory.SetItemAt (position, newItem.itemStack);
                } else if (inventory.SetItemAt (position, newItem.itemStack)) {
                    Destroy (newItem.gameObject);
                } else {
                    newItem.resetToSlot ();
                }
            }
        }
    }
}
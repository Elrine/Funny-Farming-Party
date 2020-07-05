using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInventory : MonoBehaviour {
    public abstract void subscribeUpdate (Vector2 pos, System.Action<ItemStack> callback);
    public abstract void unsubscribeUpdate (Vector2 pos, System.Action<ItemStack> callback);
    public abstract bool SetItemAt (Vector2 pos, ItemStack stack);
    public abstract void RemoveSlot (Vector2 pos);
}
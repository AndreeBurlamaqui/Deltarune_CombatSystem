using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] string _itemName;
    [SerializeField] string _itemDescription;
    public int itemQuantity;
    [SerializeField] GameEvent itemAction;

    public string ItemName => $"{_itemName}{(itemQuantity > 1 ? $" ({itemQuantity})" : "")}";
    public string ItemDescription => _itemDescription;
    public void Use()
    {
        if (itemAction == null)
        {
            Debug.Log($"No action assigned on item {_itemName}.");
            return;
        }

        // Do we have enought quantity?
        if (itemQuantity <= 0)
            return;

        // If so, reduce and call action
        itemQuantity--;
        itemAction.Raise();
    }
}

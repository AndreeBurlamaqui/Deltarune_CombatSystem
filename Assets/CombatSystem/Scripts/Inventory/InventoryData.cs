using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyCacto.Utils;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory System/Inventory Data")]
public class InventoryData : ScriptableObject
{

    [SerializeField] List<ItemData> _globalItems = new List<ItemData>();
    List<ItemData> _runtimeInventory = new List<ItemData>();

    private void OnDisable()
    {
        _runtimeInventory.Clear();
    }
    /// <summary>
    /// Get a list of all items informations
    /// <para>Duplicated items will have it's name modified by it's quantity</para>
    /// </summary>
    public List<ItemData> GetInventory()
    {

        // TIP: Remove it if you want the data to be persistent and the inventory also acts like a save file
        // Or, if you want it to act like a save file, you can wrap in #if UNITY_EDITOR
        if (_runtimeInventory.Count <= 0)
        {
            foreach (ItemData item in _globalItems)
            {
                _runtimeInventory.Add(Instantiate(item));
            }
        }

        return _runtimeInventory;
    }

    /// <summary>
    /// Add an item. If there's a duplicate, will increase it's quantity instead.
    /// </summary>
    public void AddItem(ItemData item)
    {
        if (_globalItems.Contains(item))
        {
            // If we have a duplicate, just increase it's quantity

            _globalItems.Find((i) => i == item).itemQuantity++;
            return;
        }

        // If we don't have, then add normally.
        _globalItems.Add(item);
    }


    /// <summary>
    /// Use a item by defined <paramref name="index"/>
    /// </summary>
    public void UseItem(int index)
    {
        GetInventory()[index].Use();

        // Do a refresh on items to check if some of them are <= 0 quantity
        foreach (ItemData item in GetInventory())
        {
            if (item.itemQuantity <= 0)
            {
                GetInventory().Remove(item);
                break;
            }
        }
    }

    [ExecuteInEditMode]
    [ContextMenu("FIX INVENTORY")]
    private void FixInventoryList()
    {
        List<ItemData> fixInventory = new List<ItemData>();

        foreach (ItemData i in _globalItems)
        {
            if (fixInventory.Contains(i))
                continue;
            i.itemQuantity = _globalItems.FindAll((x) => x == i).Count;
            Debug.Log($"Changing {i.ItemName}");
            fixInventory.Add(i);
        }

        if (fixInventory.Count <= 0)
            return;


        _globalItems.Clear();
        _globalItems.AddRange(fixInventory);
    }


    [ExecuteInEditMode]
    [ContextMenu("TEST LIST NAMES")]
    private void TestList()
    {
        foreach(ItemData i in _globalItems)
            Debug.Log("Getting item: " + i.ItemName);

    }


}

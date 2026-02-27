using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class GameCursorItemManager : IDisposable
{
    private InventoryManager _inventoryManager;

    private InventoryItem _cursorItem;

    [Inject]
    private void Construct(
        InventoryManager inventoryManager,
        InventoryItem cursorItem)
    {
        _inventoryManager = inventoryManager;
        _cursorItem = cursorItem;

        _cursorItem.gameObject.GetComponent<Image>().raycastTarget = false;

        _inventoryManager.OnDisableInventory += ClearItemData;
    }

    public void Dispose()
    {
        _inventoryManager.OnDisableInventory -= ClearItemData;
    }

    private void ClearItemData()
    {
        _cursorItem.SetCurrentItemData(null);
    }

    public InventoryItem GetCursorItem()
    {
        return _cursorItem;
    }
}
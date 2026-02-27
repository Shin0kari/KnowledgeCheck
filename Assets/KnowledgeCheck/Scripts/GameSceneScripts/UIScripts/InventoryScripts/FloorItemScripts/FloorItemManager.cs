using System;
using Zenject;

public class FloorItemManager : IDisposable
{
    private InventoryManager _inventoryManager;
    private InventoryItem _floorItem;

    [Inject]
    private void Construct(
        InventoryManager inventoryManager,
        InventoryItem floorItem
    )
    {
        _inventoryManager = inventoryManager;
        _floorItem = floorItem;

        _inventoryManager.OnDisableInventory += ClearItemData;
    }

    public void Dispose()
    {
        _inventoryManager.OnDisableInventory -= ClearItemData;
    }

    private void ClearItemData()
    {
        _floorItem.SetCurrentItemData(null);
    }
}
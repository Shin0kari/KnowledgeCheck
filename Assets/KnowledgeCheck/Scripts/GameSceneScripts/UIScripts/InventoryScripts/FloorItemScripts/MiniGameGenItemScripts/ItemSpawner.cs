using System;
using UnityEngine;
using Zenject;

public class FloorItemSpawner
{
    [SerializeField] private InventoryItem[] _floorItems;
    private ItemFactory _itemFactory;

    [Inject]
    private void Construct(
        ItemFactory itemFactory,
        InventoryItem[] floorItems)
    {
        _itemFactory = itemFactory;
        _floorItems = floorItems;
    }

    public void SpawnItem()
    {
        foreach (var floorItem in _floorItems)
        {
            if (floorItem.GetCurrentItemData() == null)
            {
                _itemFactory.SpawnItemOnPanel(floorItem);
                return;
            }
        }
    }
}
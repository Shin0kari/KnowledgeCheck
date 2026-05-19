using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class FloorItemSpawner
{
    private HashSet<InventoryItem> _floorItems = new();
    private ItemFactory _itemFactory;

    [Inject]
    private void Construct(ItemFactory itemFactory)
    {
        _itemFactory = itemFactory;
    }

    public void UpdateFloorItems(HashSet<InventoryItem> floorItems)
    {
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
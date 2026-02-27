using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Container Item", menuName = "Inventory/Container Item")]
public class ContainerItemSO : EquippableItemSO
{
    public List<ItemSO> containerItems = new();

    private void Awake()
    {
        _itemType = ItemType.Equipment;
        _equipSlot = EquipmentSlot.Container;
        _isContainer = true;
    }

    public int GetContainerCapacity()
    {
        return containerItems.Count;
    }

    public override Item CreateRuntimeItem()
    {
        return new ContainerItem
        {
            ItemName = GetItemName(),
            ItemStats = _itemStats,
            ItemAffects = _itemAffects,
            Durability = this.Durability,
            ContainerItems = CreateContainerItemsDictionary(containerItems)
        };
    }

    private Dictionary<int, Item> CreateContainerItemsDictionary(List<ItemSO> uiContainedItems)
    {
        Dictionary<int, Item> newContainedItems = new();

        for (int i = 0; i < uiContainedItems.Count; i++)
        {
            if (uiContainedItems[i] != null)
            {
                newContainedItems.Add(i, uiContainedItems[i].CreateRuntimeItem());
            }
        }

        return newContainedItems;
    }
}
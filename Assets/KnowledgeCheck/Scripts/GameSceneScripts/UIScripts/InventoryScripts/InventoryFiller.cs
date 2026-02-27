using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryFiller
{
    private GameObject _parentContainerInventoryPanel;
    private ItemsDB _dbItems;

    private ContainerSlotFactory _containerSlotFactory;
    private InventoryManager _inventoryManager;

    private Dictionary<string, ItemSO> _allItemsSO = new();

    public Dictionary<string, ItemSO> AllItemsSO
    {
        get
        {
            return _allItemsSO;
        }
        private set
        {
            _allItemsSO = value;
        }
    }

    [Inject]
    private void Construct(
        GameObject parentContainerInventoryPanel,
        ItemsDB dbItems,
        ContainerSlotFactory containerSlotFactory,
        InventoryManager inventoryManager)
    {
        _parentContainerInventoryPanel = parentContainerInventoryPanel;
        _dbItems = dbItems;

        _containerSlotFactory = containerSlotFactory;
        _inventoryManager = inventoryManager;

        FillDictionaryFromList(_dbItems.allItemsSO, _allItemsSO);
    }

    private void FillDictionaryFromList<T>(List<T> listItems, Dictionary<string, T> dictionaryItems) where T : ItemSO
    {
        foreach (var item in listItems)
        {
            dictionaryItems.Add(item.GetItemName(), item);
        }
    }

    private void FillItemSODictionaryFromTDictionary<T>(
        Dictionary<string, ItemSO> dictionaryItemSO,
        Dictionary<string, T> dictionaryT)
    {
        foreach (var item in dictionaryT)
        {
            dictionaryItemSO.Add(item.Key, item.Value as ItemSO);
        }
    }

    public void FillContainerInventoryFromContainerSO()
    {
        ContainerItemSO uiContainerSO = _inventoryManager.GetAdditionalItems().container.GetCurrentItemData() as ContainerItemSO;
        if (uiContainerSO == null)
        {
            if (_parentContainerInventoryPanel.transform.childCount > 0)
                ClearContainerInventory();
            return;
        }

        var createdContainerInventory = SpawnNewContainerInventory(uiContainerSO);

        foreach (var (slotIndex, ItemPanel) in createdContainerInventory)
        {
            ItemPanel.gameObject.GetComponentInChildren<InventoryItem>().SetCurrentItemData(uiContainerSO.containerItems[slotIndex]);
        }
    }

    public void FillContainerSOFromContainerInventory()
    {
        ContainerItemSO uiContainerSO = _inventoryManager.GetAdditionalItems().container.GetCurrentItemData() as ContainerItemSO;
        if (uiContainerSO == null)
            return;

        List<InventoryItem> uiContainerInventoryItems = _inventoryManager.GetContainerInventoryItems();

        List<ItemSO> containerItems = new();
        foreach (var itemSO in uiContainerInventoryItems)
        {
            containerItems.Add(itemSO.GetCurrentItemData());
        }
        uiContainerSO.containerItems = containerItems;
    }

    private List<ItemSO> ConvertContainerItemsToItemsSO(Dictionary<int, Item> expectedContainerItems, ContainerItemSO container)
    {
        Dictionary<int, ItemPanel> createdContainerInventory = SpawnNewContainerInventory(container);

        List<ItemSO> containedItems = new(); // лист предметов, которые хранятся в контейнере
        foreach (var (slotIndex, ItemPanel) in createdContainerInventory)
        {
            if (!expectedContainerItems.TryGetValue(slotIndex, out Item item))
            {
                containedItems.Add(null);
                continue;
            }

            containedItems.Add(TryFillChoicedInventoryItemFromItem(item, ItemPanel.gameObject.GetComponentInChildren<InventoryItem>()).GetCurrentItemData());
        }

        return containedItems;
    }

    private Dictionary<int, ItemPanel> SpawnNewContainerInventory(ContainerItemSO container)
    {
        ClearContainerInventory();

        Dictionary<int, ItemPanel> createdContainerInventory = new();
        for (int i = 0; i < container.GetContainerCapacity(); i++)
        {
            createdContainerInventory
                .Add(i, _containerSlotFactory.SpawnPanelOnInventory(_parentContainerInventoryPanel));
        }

        return createdContainerInventory;
    }

    public void ClearContainerInventory()
    {
        if (_parentContainerInventoryPanel.transform.childCount > 0)
        {
            _containerSlotFactory.DespawnPanelsInInventory(_parentContainerInventoryPanel);
        }
    }

    private ItemSO TryGetCopyItemSOFromItem(Item item)
    {
        if (!AllItemsSO.TryGetValue(item.ItemName, out ItemSO itemSO))
            return itemSO;

        itemSO = InventoryItem.TryReturnCloneItemData(itemSO);
        switch (itemSO)
        {
            case ConsumableItemSO:
                (itemSO as ConsumableItemSO).SetQuantity((item as ConsumableItem).Quantity);
                break;
            case ContainerItemSO:
                (itemSO as ContainerItemSO).containerItems =
                    ConvertContainerItemsToItemsSO((item as ContainerItem).ContainerItems, itemSO as ContainerItemSO);
                break;
            case EquippableItemSO:
                (itemSO as EquippableItemSO).Durability = (item as EquippableItem).Durability;
                break;
            default:
                break;
        }

        itemSO._itemStats = item.ItemStats;
        itemSO._itemAffects = item.ItemAffects;

        return itemSO;
    }

    public void FillAdditionalItems(AdditionalItems characterEAItems) // EA - EquippableAdditional
    {
        var additionalItems = _inventoryManager.GetAdditionalItems();
        TryFillChoicedInventoryItemFromItem(characterEAItems.Container, additionalItems.container);
    }

    public void FillMainItems(MainItems characterEquippableMainItems)
    {
        var mainItems = _inventoryManager.GetMainItems();
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.HeadItem, mainItems.headItem);
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.ChestItem, mainItems.chestItem);
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.LeftHandItem, mainItems.leftHandItem);
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.RightHandItem, mainItems.rightHandItem);
    }

    private InventoryItem TryFillChoicedInventoryItemFromItem<T>(T choicedItem, InventoryItem inventoryItem) where T : Item
    {
        if (choicedItem == null)
        {
            inventoryItem.SetCurrentItemData(null);
            return null;
        }

        ItemSO newItem = TryGetCopyItemSOFromItem(choicedItem);
        return TryFillChoicedInventoryItemFromItemSO(inventoryItem, newItem);
    }

    public InventoryItem TryFillChoicedInventoryItemFromItemSO(InventoryItem inventoryItem, ItemSO newItem)
    {
        if (newItem == null)
        {
            inventoryItem.SetCurrentItemData(null);
            return null;
        }

        inventoryItem.SetCurrentItemData(newItem);
        return inventoryItem;
    }
}
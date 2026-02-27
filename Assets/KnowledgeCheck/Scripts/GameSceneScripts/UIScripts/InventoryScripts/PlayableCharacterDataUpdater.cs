using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Zenject;

public class PlayableCharacterDataUpdater : IInitializable
{
    private GameData _gameData;
    private InventoryManager _inventoryManager;

    private Inventory _inventory = new();
    public event Action OnDataUpdate;

    [Inject]
    private void Construct(
        GameData gameData,
        InventoryManager inventoryManager,
        InventoryFiller inventoryFiller
    )
    {
        _gameData = gameData;
        _inventoryManager = inventoryManager;
    }

    public void Initialize()
    {
        var (saveName, saveData) = _gameData.GetCurrentGameData();
        if (saveName == null)
            return;
        _inventory = _gameData.GetCurrentGameData().saveData.Player.Inventory;
    }

    public void UpdateCharacterData()
    {
        // EI - Equippable Items
        var uiMainEI = _inventoryManager.GetMainItems();
        var uiAdditionalEI = _inventoryManager.GetAdditionalItems();
        var uiContainerInventoryItems = _inventoryManager.GetContainerInventoryItems();

        var characterMainEI = _inventory.EquippableMainItems;
        var characterAdditionalEI = _inventory.EquippableAdditionalItems;

        characterMainEI.HeadItem = ConvertItemFromItemSO(uiMainEI.headItem.GetCurrentItemData(), characterMainEI.HeadItem);
        characterMainEI.ChestItem = ConvertItemFromItemSO(uiMainEI.chestItem.GetCurrentItemData(), characterMainEI.ChestItem);
        characterMainEI.LeftHandItem = ConvertItemFromItemSO(uiMainEI.leftHandItem.GetCurrentItemData(), characterMainEI.LeftHandItem);
        characterMainEI.RightHandItem = ConvertItemFromItemSO(uiMainEI.rightHandItem.GetCurrentItemData(), characterMainEI.RightHandItem);

        characterAdditionalEI.Container = ConvertItemFromItemSO(uiAdditionalEI.container.GetCurrentItemData(), characterAdditionalEI.Container);

        OnDataUpdate?.Invoke();
    }

    private T_Item ConvertItemFromItemSO<T_SO, T_Item>(T_SO sourceItemSO, T_Item _destinationItem)
        where T_SO : ItemSO
        where T_Item : Item
    {
        if (sourceItemSO == null) return null;

        return sourceItemSO.CreateRuntimeItem() as T_Item;
    }
}
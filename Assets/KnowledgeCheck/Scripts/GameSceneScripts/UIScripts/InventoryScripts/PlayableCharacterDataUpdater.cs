using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Profiling;
using Zenject;

public class PlayableCharacterDataUpdater : IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private IGetGameData _gameData;
    private InventoryManager _inventoryManager;

    private Inventory _inventory = new();
    public event Action OnDataUpdate;

    private DisposableBag _dB;
    private UniTaskCompletionSource _inventoryManagerLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IGetGameData gameData,
        IAssetProviderGetter assetProvider
    )
    {
        _assetProvider = assetProvider;
        _gameData = gameData;

        SubscribeOnUpdateObjects();
        _gameData.CurrentSaveUpdated += SetInventoryFromCurrentSave;
    }

    public void Dispose()
    {
        if (_gameData != null)
            _gameData.CurrentSaveUpdated -= SetInventoryFromCurrentSave;

        _inventoryManagerLoadedSource.TrySetCanceled();
        _inventoryManagerLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();

        OnDataUpdate = null;
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<InventoryManager>()
            .OfType<IBindingSingletonComponent, InventoryManager>()
            .Subscribe(inventoryManager =>
            {
                if (inventoryManager == null)
                    return;

                _inventoryManager = inventoryManager;
                _inventoryManagerLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void SetInventoryFromCurrentSave()
    {
        var (uuid, saveData) = _gameData.GetCurrentGameData();
        if (uuid == null)
            return;
        _inventory = saveData.Player.Inventory;
    }

    public async UniTask UpdateCharacterData()
    {
        await _inventoryManagerLoadedSource.Task.AttachExternalCancellation(_ct.Token);
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
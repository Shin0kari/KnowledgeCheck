using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class InventoryFiller : IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private ItemsDB _dbItems;
    // private ParentContainerInventoryPanelLinker _parentContainerLinker;

    private GameObject _parentContainerInventoryPanel;

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

    private DisposableBag _dB;
    private UniTaskCompletionSource _containerPanelSource = new();
    private UniTaskCompletionSource _inventoryManagerLoadedSource = new();
    private UniTaskCompletionSource _dbItemsLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        ContainerSlotFactory containerSlotFactory
    )
    {
        _assetProvider = assetProvider;
        _containerSlotFactory = containerSlotFactory;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        _containerPanelSource.TrySetCanceled();
        _containerPanelSource = null;

        _inventoryManagerLoadedSource.TrySetCanceled();
        _inventoryManagerLoadedSource = null;

        _dbItemsLoadedSource.TrySetCanceled();
        _dbItemsLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();

        _allItemsSO.Clear();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ItemsDB>()
            .OfType<IBindingSingletonComponent, ItemsDB>()
            .Subscribe(dbItems =>
            {
                if (dbItems == null)
                    return;

                _dbItems = dbItems;
                FillDictionaryFromList(_dbItems.allItemsSO, ref _allItemsSO);
                _dbItemsLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

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

        _assetProvider
            .GetIBindingSingletonComponent<ParentContainerInventoryPanelLinker>()
            .OfType<IBindingSingletonComponent, ParentContainerInventoryPanelLinker>()
            .Subscribe(parentContainerLinker =>
            {
                if (parentContainerLinker == null)
                    return;

                _parentContainerInventoryPanel = parentContainerLinker.LinkerObject;
                _containerPanelSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void FillDictionaryFromList<T>(List<T> listItems, ref Dictionary<string, T> dictionaryItems) where T : ItemSO
    {
        dictionaryItems.Clear();
        foreach (var item in listItems)
        {
            dictionaryItems.Add(item.GetItemName(), item);
        }
    }

    // private void FillItemSODictionaryFromTDictionary<T>(
    //     Dictionary<string, ItemSO> dictionaryItemSO,
    //     Dictionary<string, T> dictionaryT)
    // {
    //     foreach (var item in dictionaryT)
    //     {
    //         dictionaryItemSO.Add(item.Key, item.Value as ItemSO);
    //     }
    // }

    // public void SetBackpackContainerPanel(GameObject parentBackPackPanel)
    // {
    //     _parentContainerInventoryPanel = parentBackPackPanel;

    //     _containerPanelSource.TrySetResult();
    // }

    public async UniTask FillContainerInventoryFromContainerSO()
    {
        await _containerPanelSource.Task.AttachExternalCancellation(_ct.Token);
        if (_parentContainerInventoryPanel == null) return;

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            _parentContainerInventoryPanel.GetCancellationTokenOnDestroy()
        );

        await _inventoryManagerLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        ContainerItemSO uiContainerSO = _inventoryManager.GetAdditionalItems().container.GetCurrentItemData() as ContainerItemSO;
        if (uiContainerSO == null)
        {
            if (_parentContainerInventoryPanel.transform.childCount > 0)
                await ClearContainerInventory(linkedCTS.Token);
            return;
        }

        var createdContainerInventory = await SpawnNewContainerInventory(uiContainerSO, linkedCTS.Token);

        foreach (var (slotIndex, ItemPanel) in createdContainerInventory)
        {
            ItemPanel.GetInventoryItem().SetCurrentItemData(uiContainerSO.containerItems[slotIndex]);
        }
    }

    public async UniTask FillContainerSOFromContainerInventory()
    {
        await _containerPanelSource.Task.AttachExternalCancellation(_ct.Token);
        if (_parentContainerInventoryPanel == null) return;

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            _parentContainerInventoryPanel.GetCancellationTokenOnDestroy()
        );

        await _inventoryManagerLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

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

    private async UniTask<List<ItemSO>> ConvertContainerItemsToItemsSO(Dictionary<int, Item> expectedContainerItems, ContainerItemSO container)
    {
        await _containerPanelSource.Task.AttachExternalCancellation(_ct.Token);
        if (_parentContainerInventoryPanel == null) return null;

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            _parentContainerInventoryPanel.GetCancellationTokenOnDestroy()
        );

        Dictionary<int, ItemPanel> createdContainerInventory = await SpawnNewContainerInventory(container, linkedCTS.Token);

        List<ItemSO> containedItems = new(); // лист предметов, которые хранятся в контейнере
        foreach (var (slotIndex, ItemPanel) in createdContainerInventory)
        {
            if (!expectedContainerItems.TryGetValue(slotIndex, out Item item))
            {
                containedItems.Add(null);
                continue;
            }

            containedItems.Add((await TryFillChoicedInventoryItemFromItem(item, ItemPanel.GetInventoryItem())).GetCurrentItemData());
        }

        return containedItems;
    }

    private async UniTask<Dictionary<int, ItemPanel>> SpawnNewContainerInventory(ContainerItemSO container, CancellationToken linkedCT)
    {
        await ClearContainerInventory(linkedCT);

        Dictionary<int, ItemPanel> createdContainerInventory = new();
        for (int i = 0; i < container.GetContainerCapacity(); i++)
        {
            var panel = await _containerSlotFactory.SpawnPanelOnInventory(_parentContainerInventoryPanel, linkedCT);
            createdContainerInventory
                .Add(i, panel);
        }

        return createdContainerInventory;
    }

    public async UniTask ClearContainerInventory(CancellationToken linkedCT)
    {
        await _containerPanelSource.Task.AttachExternalCancellation(_ct.Token);
        if (_parentContainerInventoryPanel == null) return;

        if (_parentContainerInventoryPanel.transform.childCount > 0)
        {
            await _containerSlotFactory.DespawnPanelsInInventory(_parentContainerInventoryPanel, linkedCT);
        }
    }

    private async UniTask<ItemSO> TryGetCopyItemSOFromItem(Item item)
    {
        await _dbItemsLoadedSource.Task.AttachExternalCancellation(_ct.Token);
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
                    await ConvertContainerItemsToItemsSO((item as ContainerItem).ContainerItems, itemSO as ContainerItemSO);
                break;
            case EquippableItemSO:
                (itemSO as EquippableItemSO).Durability = (item as EquippableItem).Durability;
                break;
            default:
                break;
        }

        itemSO.ItemStats = item.ItemStats;
        itemSO.ItemAffects = item.ItemAffects;

        return itemSO;
    }

    public async UniTask FillAdditionalItems(AdditionalItems characterEAItems) // EA - EquippableAdditional
    {
        await _inventoryManagerLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        var additionalItems = _inventoryManager.GetAdditionalItems();
        TryFillChoicedInventoryItemFromItem(characterEAItems.Container, additionalItems.container).Forget();
    }

    public async UniTask FillMainItems(MainItems characterEquippableMainItems)
    {
        await _inventoryManagerLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        var mainItems = _inventoryManager.GetMainItems();
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.HeadItem, mainItems.headItem).Forget();
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.ChestItem, mainItems.chestItem).Forget();
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.LeftHandItem, mainItems.leftHandItem).Forget();
        TryFillChoicedInventoryItemFromItem(characterEquippableMainItems.RightHandItem, mainItems.rightHandItem).Forget();
    }

    private async UniTask<InventoryItem> TryFillChoicedInventoryItemFromItem<T>(T choicedItem, InventoryItem inventoryItem) where T : Item
    {
        if (choicedItem == null)
        {
            inventoryItem.SetCurrentItemData(null);
            return null;
        }

        ItemSO newItem = await TryGetCopyItemSOFromItem(choicedItem);
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
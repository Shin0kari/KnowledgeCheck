using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Zenject;

public class FloorItemManager : IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private InventoryManager _inventoryManager;

    private FloorItemSpawner _floorItemSpawner;

    private HashSet<InventoryItem> _floorItems = new();

    private DisposableBag _dB;
    private DisposableBag _iDB;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        FloorItemSpawner floorItemSpawner)
    {
        _assetProvider = assetProvider;
        _floorItemSpawner = floorItemSpawner;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        // if (_inventoryManager != null)
        //     _inventoryManager.OnDisableInventory -= ClearItemData;

        _dB.Dispose();
        _iDB.Dispose();

        _floorItems?.Clear();
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
                // if (_inventoryManager != null)
                //     _inventoryManager.OnDisableInventory -= ClearItemData;

                _inventoryManager = inventoryManager;
                _inventoryManager.InventoryState.Subscribe(state =>
                {
                    if (!state) ClearItemData();
                }).AddTo(ref _iDB);
                // _inventoryManager.OnDisableInventory += ClearItemData;
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingTransientComponent<FloorItemLinker>()
            .Subscribe(floorItemLinkers =>
            {
                if (floorItemLinkers == null || floorItemLinkers.Count < 1)
                    return;

                SetFloorItems(floorItemLinkers);
            })
            .AddTo(ref _dB);
    }

    private void SetFloorItems(List<IBindingTransientComponent> floorItemLinkers)
    {
        foreach (FloorItemLinker floorItemLinker in floorItemLinkers.Cast<FloorItemLinker>())
        {
            if (!_floorItems.Contains(floorItemLinker.LinkerObject))
            {
                _floorItems.Add(floorItemLinker.LinkerObject);
            }
        }
        _floorItemSpawner.UpdateFloorItems(_floorItems);
    }

    private void ClearItemData()
    {
        foreach (var item in _floorItems)
        {
            item.SetCurrentItemData(null);
        }
    }
}
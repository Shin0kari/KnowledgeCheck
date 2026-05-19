using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class FollowToCursor : MonoBehaviour
{
    private IAssetProviderGetter _assetProvider;
    private InventoryManager _inventoryManager;

    private bool isFollowEnabled = false;

    private Mouse _currentMouse;

    private DisposableBag _dB;
    private DisposableBag _iDB;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        _currentMouse = Mouse.current;

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {

        // if (_inventoryManager != null)
        // {
        //     // _inventoryManager.OnEnableInventory -= EnableFollow;
        //     // _inventoryManager.OnDisableInventory -= DisableFollow;
        // }

        _dB.Dispose();
        _iDB.Dispose();
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

                _iDB.Dispose();
                _iDB = new();
                // if (_inventoryManager != null)
                // {
                //     _inventoryManager.OnEnableInventory -= EnableFollow;
                //     _inventoryManager.OnDisableInventory -= DisableFollow;
                // }

                _inventoryManager = inventoryManager;

                // _inventoryManager.OnEnableInventory += EnableFollow;
                // _inventoryManager.OnDisableInventory += DisableFollow;

                _inventoryManager.InventoryState.Subscribe(state =>
                {
                    if (state) EnableFollow();
                    else DisableFollow();
                }).AddTo(ref _iDB);
            })
            .AddTo(ref _dB);
    }

    private void Update()
    {
        if (isFollowEnabled)
        {
            transform.position = _currentMouse.position.ReadValue();
        }
    }

    private void EnableFollow()
    {
        isFollowEnabled = true;
    }

    private void DisableFollow()
    {
        isFollowEnabled = false;
    }
}
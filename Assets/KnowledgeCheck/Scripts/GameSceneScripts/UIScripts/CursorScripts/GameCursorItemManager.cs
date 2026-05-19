using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.UI;
using Zenject;

public class GameCursorItemManager : IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private InventoryManager _inventoryManager;

    private InventoryItem _cursorItem;

    public event Action<InventoryItem> OnCursorLoaded;

    private DisposableBag _dB;
    private DisposableBag _iDB;
    private UniTaskCompletionSource _cursorItemLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        _cursorItemLoadedSource.TrySetCanceled();
        _cursorItemLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
        _iDB.Dispose();

        OnCursorLoaded = null;
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
                //     _inventoryManager.OnDisableInventory -= ClearItemData;

                _inventoryManager = inventoryManager;
                // _inventoryManager.OnDisableInventory += ClearItemData;

                _inventoryManager.InventoryState.Subscribe(state =>
                {
                    if (!state) ClearItemData();
                }).AddTo(ref _iDB);
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<CursorItemLinker>()
            .OfType<IBindingSingletonComponent, CursorItemLinker>()
            .Subscribe(cursorItemLinker =>
            {
                if (cursorItemLinker == null)
                    return;

                SetCursorItem(cursorItemLinker.LinkerObject.GetComponent<InventoryItem>());
                _cursorItemLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    public void SetCursorItem(InventoryItem cursorItem)
    {
        _cursorItem = cursorItem;
        _cursorItem.gameObject.GetComponent<Image>().raycastTarget = false;
        OnCursorLoaded?.Invoke(_cursorItem);
    }

    private void ClearItemData()
    {
        AsyncClearItemData().Forget();
    }

    private async UniTask AsyncClearItemData()
    {
        try
        {
            await _cursorItemLoadedSource.Task.AttachExternalCancellation(_ct.Token);
            _cursorItem?.SetCurrentItemData(null);
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    public InventoryItem GetCursorItem()
    {
        return _cursorItem;
    }
}
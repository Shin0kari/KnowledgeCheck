using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

public class ContainerSlotFactory : AbstractFactoryStarter, IDisposable
{
    private BaseUIItemInfoProvider _baseUIItemInfoProvider;
    private IAddressablesProvider _aP; // addressablesProvider

    private ReactiveProperty<ScriptableObject> _reactiveBaseUIItemInfoSO;
    private BaseUIItemInfoSO _baseUIItemInfoSO;

    private GameObject _itemPanelPrefab;

    private ItemPanel.Factory _itemPanelFactory;

    private DisposableBag _dB;
    private UniTaskCompletionSource _uiItemInfoLoadedSource = new();
    private UniTaskCompletionSource _itemPanelPrefabLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAddressablesProvider aP,
        ItemPanel.Factory itemPanelFactory,
        BaseUIItemInfoProvider baseUIItemInfoProvider
    )
    {
        _aP = aP;
        _itemPanelFactory = itemPanelFactory;
        _baseUIItemInfoProvider = baseUIItemInfoProvider;

        AsyncLoadResource().Forget();
        AsyncSetEnemyPrefab().Forget();
    }

    public void Dispose()
    {
        _uiItemInfoLoadedSource.TrySetCanceled();
        _uiItemInfoLoadedSource = null;

        _itemPanelPrefabLoadedSource.TrySetCanceled();
        _itemPanelPrefabLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private async UniTask AsyncLoadResource()
    {
        _reactiveBaseUIItemInfoSO = await _baseUIItemInfoProvider.TryGetDataSO(_ct.Token);

        _reactiveBaseUIItemInfoSO?
            .Subscribe(baseUIItemInfoSO =>
            {
                if (baseUIItemInfoSO == null)
                    return;

                SetSO(baseUIItemInfoSO);
            })
            .AddTo(ref _dB);
    }

    private void SetSO(ScriptableObject baseUIItemInfoSO)
    {
        if (baseUIItemInfoSO is not BaseUIItemInfoSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _baseUIItemInfoSO = so;
        _uiItemInfoLoadedSource.TrySetResult();
    }

    public async UniTask AsyncSetEnemyPrefab()
    {
        await _uiItemInfoLoadedSource.Task.AttachExternalCancellation(_ct.Token);
        _itemPanelPrefab = await _aP.AsyncGetAddressablesDataFromReference<GameObject>(_baseUIItemInfoSO.ItemPanelPrefab, _ct.Token);
        _itemPanelPrefabLoadedSource.TrySetResult();
    }

    public async UniTask<ItemPanel> SpawnPanelOnInventory(GameObject inventoryPanel, CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, ct);
        await _itemPanelPrefabLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        var itemPanel = _itemPanelFactory.Create(_itemPanelPrefab);
        itemPanel.transform.SetParent(inventoryPanel.transform);
        itemPanel.transform.localScale = new(1f, 1f, 1f);
        return itemPanel;
    }

    public async UniTask DespawnPanelsInInventory(GameObject inventoryPanel, CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, ct);

        for (int i = 0; i < inventoryPanel.transform.childCount;)
        {
            var itemPanel = inventoryPanel.transform.GetChild(i).gameObject;
            UnityEngine.Object.Destroy(itemPanel);
            await UniTask.Yield(cancellationToken: linkedCTS.Token);
        }
    }
}
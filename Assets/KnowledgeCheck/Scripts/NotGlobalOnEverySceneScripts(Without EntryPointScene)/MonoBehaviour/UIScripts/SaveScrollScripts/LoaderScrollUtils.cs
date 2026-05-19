using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class LoaderScrollUtils : AbstractScrollUtils
{
    private LoadMenuSavePanelsProvider _loadMenuSavePanelsProvider;
    private LoadMenuSavePanelsSO _loadMenuSavePanelsSO;

    [Inject]
    private void Construct(
        LoadMenuSavePanelsProvider loadMenuSavePanelsProvider,
        IAssetProviderGetter assetProvider
    )
    {
        _loadMenuSavePanelsProvider = loadMenuSavePanelsProvider;
        _assetProvider = assetProvider;

        BindAllTypes();

        _ct = gameObject.GetCancellationTokenOnDestroy();

        LoadSavePanelPrefabs().Forget();
    }

    protected override async UniTaskVoid LoadSavePanelPrefabs()
    {
        _reactiveLoadSavePanelsSO = await _loadMenuSavePanelsProvider.TryGetDataSO(_ct);

        _reactiveLoadSavePanelsSO?
            .Subscribe((loadMenuSavePanelsSO) =>
            {
                if (loadMenuSavePanelsSO == null)
                    return;

                SetSO(loadMenuSavePanelsSO).Forget();
            })
            .AddTo(ref _dB);
    }

    protected override async UniTask SetSO(ScriptableObject loadMenuSavePanelsSO)
    {
        _ctSO?.Cancel();
        _ctSO?.Dispose();
        _iDB.Dispose();

        _ctSO = new();
        _iDB = new();

        if (loadMenuSavePanelsSO is not LoadMenuSavePanelsSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _loadMenuSavePanelsSO = so;

        await LoadSOResources(_loadMenuSavePanelsSO, _ctSO.Token);
    }

    private async UniTask LoadSOResources(LoadMenuSavePanelsSO loadMenuSavePanelsSO, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct,
            ct
        );

        _reactiveSavePanelPrefab = await GetReactiveGameObject(loadMenuSavePanelsSO.LoadPanel, linkedCTS.Token);
        _reactiveNewSaveButtonPrefab = await GetReactiveGameObject(loadMenuSavePanelsSO.NewGamePanel, linkedCTS.Token);

        _reactiveSavePanelPrefab?
            .Subscribe((prefab) =>
            {
                if (prefab == null) return;
                _savePanelPrefab = prefab;
                _loadedSavePanel.TrySetResult();
            }).AddTo(ref _iDB);

        _reactiveNewSaveButtonPrefab?
            .Subscribe((prefab) =>
            {
                if (prefab == null) return;
                _newSaveButtonPrefab = prefab;
                _loadedNewSaveButton.TrySetResult();
            }).AddTo(ref _iDB);
    }

}
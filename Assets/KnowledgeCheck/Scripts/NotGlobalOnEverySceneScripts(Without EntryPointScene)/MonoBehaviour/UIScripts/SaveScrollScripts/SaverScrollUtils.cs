using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class SaverScrollUtils : AbstractScrollUtils
{
    private SaveMenuSavePanelsProvider _saveMenuSavePanelsProvider;
    private SaveMenuSavePanelsSO _saveMenuSavePanelsSO;

    [Inject]
    private void Construct(
        SaveMenuSavePanelsProvider saveMenuSavePanelsProvider,
        IAssetProviderGetter assetProvider
    )
    {
        _saveMenuSavePanelsProvider = saveMenuSavePanelsProvider;
        _assetProvider = assetProvider;

        BindAllTypes();

        _ct = gameObject.GetCancellationTokenOnDestroy();

        LoadSavePanelPrefabs().Forget();
    }

    protected override async UniTaskVoid LoadSavePanelPrefabs()
    {
        _reactiveLoadSavePanelsSO = await _saveMenuSavePanelsProvider.TryGetDataSO(_ct);

        _reactiveLoadSavePanelsSO?
            .Subscribe((saveMenuSavePanelsSO) =>
            {
                if (saveMenuSavePanelsSO == null)
                    return;

                SetSO(saveMenuSavePanelsSO).Forget();
            })
            .AddTo(ref _dB);
    }

    protected override async UniTask SetSO(ScriptableObject saveMenuSavePanelsSO)
    {
        _ctSO?.Cancel();
        _ctSO?.Dispose();
        _iDB.Dispose();

        _ctSO = new();
        _iDB = new();

        if (saveMenuSavePanelsSO is not SaveMenuSavePanelsSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _saveMenuSavePanelsSO = so;

        await LoadSOResources(_saveMenuSavePanelsSO, _ctSO.Token);
    }

    private async UniTask LoadSOResources(SaveMenuSavePanelsSO saveMenuSavePanelsSO, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct,
            ct
        );

        _reactiveSavePanelPrefab = await GetReactiveGameObject(saveMenuSavePanelsSO.SavePanel, linkedCTS.Token);
        _reactiveNewSaveButtonPrefab = await GetReactiveGameObject(saveMenuSavePanelsSO.NewSavePanel, linkedCTS.Token);

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
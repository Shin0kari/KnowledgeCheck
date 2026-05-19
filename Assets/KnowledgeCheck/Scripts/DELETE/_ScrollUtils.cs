using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Zenject;

public class ScrollUtils : MonoBehaviour, IScrollUtils
{
    [SerializeField] private ScrollRect _scroll;

    protected IAssetProviderGetter _assetProvider;
    private LoadMenuSavePanelsProvider _loadMenuSavePanelsProvider;

    protected ReactiveProperty<ScriptableObject> _reactiveLoadSavePanelsSO;
    private LoadMenuSavePanelsSO _loadMenuSavePanelsSO;

    protected ReadOnlyReactiveProperty<GameObject> _reactiveSavePanelPrefab;
    protected ReadOnlyReactiveProperty<GameObject> _reactiveNewSaveButtonPrefab;
    protected GameObject _savePanelPrefab; protected GameObject _newSaveButton;

    protected DisposableBag _dB;
    protected DisposableBag _iDB;

    protected UniTaskCompletionSource _loadedSavePanel = new();
    protected UniTaskCompletionSource _loadedNewSaveButton = new();

    protected CancellationTokenSource _ctSO = new();
    protected CancellationToken _ct;

    [Inject]
    protected virtual void Construct(
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

    private void OnDestroy()
    {
        _loadedSavePanel?.TrySetCanceled();
        _loadedSavePanel = null;

        _loadedNewSaveButton?.TrySetCanceled();
        _loadedNewSaveButton = null;

        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _iDB.Dispose();
        _dB.Dispose();
    }

    private async UniTaskVoid LoadSavePanelPrefabs()
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

    private async UniTask SetSO(ScriptableObject loadMenuSavePanelsSO)
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
                _newSaveButton = prefab;
                _loadedNewSaveButton.TrySetResult();
            }).AddTo(ref _iDB);
    }

    protected async UniTask<ReadOnlyReactiveProperty<GameObject>> GetReactiveGameObject(AssetReferenceT<GameObject> prefab, CancellationToken ct)
    {
        if (_assetProvider == null)
            return null;

        return (await _assetProvider.GetSharedResourceData(prefab, ct))
                    .OfType<UnityEngine.Object, GameObject>()
                    .ToReadOnlyReactiveProperty();
    }

    public void SetActiveStateForNewSaveButton(bool state)
    {
        if (_newSaveButton == null) return;
        _newSaveButton.SetActive(state);
    }

    public GameObject GetScrollChildGameObject(int childIndex) => _scroll.content.GetChild(childIndex).gameObject;

    public int GetCountSaves()
    {
        if (_newSaveButton == null) return 0;
        return _newSaveButton.activeSelf ?
            _scroll.content.childCount - 1 :
            _scroll.content.childCount;
    }

    public int GetCountContent() => _scroll.content.childCount;

    public List<GameObject> GetAllContent()
    {
        List<GameObject> allContent = new();

        for (int i = 1; i < GetCountContent(); i++)
        {
            allContent.Add(_scroll.content.GetChild(i).gameObject);
        }

        return allContent;
    }

    public ScrollRect GetScroll() => _scroll;
    public async UniTask<GameObject> GetSavePrefab(CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct, ct);

        try
        {
            await _loadedSavePanel.Task.AttachExternalCancellation(linkedCTS.Token);
            return _savePanelPrefab;
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }
    }
    public async UniTask<GameObject> GetNewSaveButton(CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct, ct);

        try
        {
            await _loadedNewSaveButton.Task.AttachExternalCancellation(linkedCTS.Token);
            return _newSaveButton;
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}
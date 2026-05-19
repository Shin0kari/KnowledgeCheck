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

public abstract class AbstractScrollUtils : MonoBehaviour, IScrollUtils
{
    [SerializeField] private ScrollRect _scroll;

    private DiContainer _container;

    protected IAssetProviderGetter _assetProvider;

    protected ReactiveProperty<ScriptableObject> _reactiveLoadSavePanelsSO;

    protected ReadOnlyReactiveProperty<GameObject> _reactiveSavePanelPrefab;
    protected ReadOnlyReactiveProperty<GameObject> _reactiveNewSaveButtonPrefab;
    protected GameObject _savePanelPrefab;
    protected GameObject _newSaveButtonPrefab;
    protected GameObject _newSaveButton;

    protected DisposableBag _dB;
    protected DisposableBag _iDB;

    protected UniTaskCompletionSource _loadedSavePanel = new();
    protected UniTaskCompletionSource _loadedNewSaveButton = new();
    protected UniTaskCompletionSource _spawnedNewSaveButton = new();

    protected CancellationTokenSource _ctSO = new();
    protected CancellationToken _ct;

    [Inject]
    private void Construct(DiContainer container)
    {
        _container = container;
    }

    private void OnDestroy()
    {
        _loadedSavePanel?.TrySetCanceled();
        _loadedSavePanel = null;

        _loadedNewSaveButton?.TrySetCanceled();
        _loadedNewSaveButton = null;

        _spawnedNewSaveButton?.TrySetCanceled();
        _spawnedNewSaveButton = null;

        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _iDB.Dispose();
        _dB.Dispose();
    }

    private void Awake()
    {
        AsyncSpawnNewSaveButton().Forget();
    }

    private async UniTask AsyncSpawnNewSaveButton()
    {
        await _loadedNewSaveButton.Task.AttachExternalCancellation(_ctSO.Token);
        if (_newSaveButtonPrefab == null || _scroll == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "_newSaveButtonPrefab or _scroll is null");

        _newSaveButton = _container.InstantiatePrefab(_newSaveButtonPrefab, _scroll.content);
        _newSaveButton.transform.localScale = new(1f, 1f, 1f);
        _newSaveButton.transform.SetAsFirstSibling();

        _spawnedNewSaveButton.TrySetResult();
    }

    protected abstract UniTaskVoid LoadSavePanelPrefabs();

    protected abstract UniTask SetSO(ScriptableObject loadMenuSavePanelsSO);

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
            await _spawnedNewSaveButton.Task.AttachExternalCancellation(linkedCTS.Token);
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
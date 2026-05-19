using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class LoadChildAddressablesGameObjects : MonoBehaviour
{
    [SerializeField] protected List<AssetReferenceT<GameObject>> _loadedAssets;

    protected DiContainer _container;
    protected IAssetProviderSpawner _assetProviderSpawner;

    protected List<GameObject> _uploadedGO = new();

    protected UniTaskCompletionSource _containerThrowed = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(DiContainer container, IAssetProviderSpawner assetProviderSpawner)
    {
        _container = container;
        _assetProviderSpawner = assetProviderSpawner;

        _uploadedGO.Capacity = _loadedAssets.Count;
        _ct = gameObject.GetCancellationTokenOnDestroy();

        _containerThrowed?.TrySetResult();
    }

    private void OnDestroy()
    {
        _containerThrowed?.TrySetCanceled();
        _containerThrowed = null;
    }

    private void Start()
    {
        LoadAllElements(_ct).Forget();
    }

    protected virtual async UniTask LoadAllElements(CancellationToken ct)
    {
        try
        {
            await _containerThrowed.Task.AttachExternalCancellation(ct);
            _uploadedGO = await _assetProviderSpawner.SpawnPrefabAssets(_loadedAssets, transform, _container, ct);
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }
}
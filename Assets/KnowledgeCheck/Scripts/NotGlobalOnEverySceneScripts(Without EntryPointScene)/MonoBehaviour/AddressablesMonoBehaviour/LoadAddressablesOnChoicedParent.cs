using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadAddressablesOnChoicedParent : LoadChildAddressablesGameObjects
{
    [SerializeField] private Transform _parentTransformLoadedAddressables;
    protected override async UniTask LoadAllElements(CancellationToken ct)
    {
        try
        {
            await _containerThrowed.Task.AttachExternalCancellation(ct);
            _uploadedGO = await _assetProviderSpawner
                .SpawnPrefabAssets(
                    _loadedAssets,
                    _parentTransformLoadedAddressables,
                    _container,
                    ct
                );
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }
}
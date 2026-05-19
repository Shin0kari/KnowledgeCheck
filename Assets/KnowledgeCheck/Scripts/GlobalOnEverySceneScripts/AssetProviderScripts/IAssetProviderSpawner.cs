using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public interface IAssetProviderSpawner
{
    public UniTask<List<GameObject>> SpawnPrefabAssets(List<AssetReferenceT<GameObject>> spawnableAssets, Transform parentTransform, DiContainer contextContainer, CancellationToken ct);
}

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressablesDataProvider : IAddressablesProvider, IDisposable
{
    private Dictionary<IResourceLocation, AsyncOperationHandle> _completedResourceOperations = new();
    private Dictionary<AssetReference, AsyncOperationHandle> _completedReferenceOperations = new();

    private CancellationTokenSource _ct = new();

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();

        foreach (var handle in _completedReferenceOperations.Values)
            handle.Release();

        foreach (var handle in _completedResourceOperations.Values)
            handle.Release();

        _completedReferenceOperations.Clear();
        _completedResourceOperations.Clear();
    }

    public void ReleaseReference(AssetReference reference)
    {
        if (_completedReferenceOperations.TryGetValue(reference, out var handle))
        {
            Addressables.Release(handle);
            _completedReferenceOperations.Remove(reference);
        }
    }

    public void ReleaseResource(IResourceLocation resource)
    {
        if (_completedResourceOperations.TryGetValue(resource, out var handle))
        {
            Addressables.Release(handle);
            _completedResourceOperations.Remove(resource);
        }
    }

    public virtual async UniTask<T> AsyncGetAddressablesDataFromLocation<T>(IResourceLocation location, CancellationToken ct) where T : UnityEngine.Object
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        if (_completedResourceOperations.TryGetValue(location, out AsyncOperationHandle asyncOperationHandle))
            return await asyncOperationHandle.Convert<T>().ToUniTask(cancellationToken: linkedCTS.Token);

        var aOH = Addressables.LoadAssetAsync<T>(location); // aOH = asyncOperationHandle
        _completedResourceOperations.Add(location, aOH);

        var taskData = await aOH.ToUniTask(cancellationToken: linkedCTS.Token);
        if (taskData == null)
        {
            _completedResourceOperations.Remove(location);
            ErrorMessageGenerator.GenerateSimpleError(this, "Addressables Prefab not found");
        }

        return taskData;
    }

    public virtual async UniTask<T> AsyncGetAddressablesDataFromReference<T>(AssetReference reference, CancellationToken ct) where T : UnityEngine.Object
    {
        if (_completedReferenceOperations.TryGetValue(reference, out AsyncOperationHandle asyncOperationHandle))
            return await asyncOperationHandle.Convert<T>().ToUniTask(cancellationToken: ct);

        var aOH = reference.LoadAssetAsync<T>();
        _completedReferenceOperations.Add(reference, aOH);

        var taskData = await aOH.ToUniTask(cancellationToken: ct);
        if (taskData == null)
        {
            _completedReferenceOperations.Remove(reference);
            ErrorMessageGenerator.GenerateSimpleError(this, "Addressables Prefab not found");
        }

        return taskData;
    }
}
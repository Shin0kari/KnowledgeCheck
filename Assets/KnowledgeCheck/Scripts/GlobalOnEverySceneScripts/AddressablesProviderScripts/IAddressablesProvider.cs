using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public interface IAddressablesProvider
{
    public UniTask<T> AsyncGetAddressablesDataFromLocation<T>(IResourceLocation location, CancellationToken ct) where T : Object;
    public UniTask<T> AsyncGetAddressablesDataFromReference<T>(AssetReference reference, CancellationToken ct) where T : Object;

    public void ReleaseReference(AssetReference reference);
    public void ReleaseResource(IResourceLocation resource);
}
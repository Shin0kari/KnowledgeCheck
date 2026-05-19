using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;
using static SceneUtils;

public class ResourceLocationProvider : IResourceLocationProvider, IDisposable
{
    private Dictionary<string, IResourceLocation> _uploadResourceLocation = new();
    private StaticResourceProvider _staticResourceProvider;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(StaticResourceProvider staticResourceProvider)
    {
        _staticResourceProvider = staticResourceProvider;
    }

    public void Dispose()
    {
        _uploadResourceLocation.Clear();

        _ct?.Cancel();
        _ct?.Dispose();
    }

    public async UniTask<IResourceLocation> GetSceneResourceLocation(SceneNames sceneName, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        var rL = await _staticResourceProvider.GetSceneLocation(sceneName, linkedCTS.Token);

        if (rL == null)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "SceneResourceLocation not found");
        }

        return rL;
    }

    public async UniTask<IResourceLocation> AsyncGetUploadResourceLocation(string prefabName, CancellationToken ct)
    {
        if (_uploadResourceLocation.TryGetValue(prefabName, out IResourceLocation resourceLocation))
        {
            if (resourceLocation == null)
            {
                ErrorMessageGenerator.GenerateSimpleError(this, "Prefab not found");
            }
            return resourceLocation;
        }

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        var rL = await Addressables.LoadResourceLocationsAsync(prefabName).WithCancellation(linkedCTS.Token);
        if (rL == null || rL.Count < 1)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Prefab not found");
        }

        _uploadResourceLocation.Add(prefabName, rL.First());

        return rL.First();
    }
}

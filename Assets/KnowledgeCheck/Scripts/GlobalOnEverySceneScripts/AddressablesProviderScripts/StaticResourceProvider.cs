using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;
using static SceneUtils;

public class StaticResourceProvider : IStaticResourceProvider, IDisposable
{
    private readonly Dictionary<SceneNames, IResourceLocation> _sceneLocations = new();

    private UniTaskCompletionSource _loadSceneSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct()
    {
        LoadSceneLocations().Forget();
    }

    public void Dispose()
    {
        _loadSceneSource.TrySetCanceled();
        _loadSceneSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _sceneLocations.Clear();
    }

    private async UniTaskVoid LoadSceneLocations()
    {
        var sceneValues = (SceneNames[])Enum.GetValues(typeof(SceneNames));

        try
        {
            var tasks = sceneValues.Select(async scene =>
            {
                var locations = await Addressables.LoadResourceLocationsAsync(scene.ToString()).ToUniTask(cancellationToken: _ct.Token);

                if (locations != null && locations.Count > 0)
                {
                    _sceneLocations[scene] = locations[0];
                }
            });

            await UniTask.WhenAll(tasks).AttachExternalCancellation(_ct.Token);
            _loadSceneSource.TrySetResult();
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    public async UniTask<IResourceLocation> GetSceneLocation(SceneNames sceneName, CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        await _loadSceneSource.Task.AttachExternalCancellation(linkedCTS.Token);

        return _sceneLocations.GetValueOrDefault(sceneName);
    }
}

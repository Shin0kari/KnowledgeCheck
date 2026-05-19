using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader : ISceneLoader, IDisposable
{
    private SceneInstance _currentScene;
    private SceneInstance _oldScene;

    private CancellationTokenSource _ct = new();

    public void Dispose()
    {
        AsyncDispose().Forget();
    }

    private async UniTask AsyncDispose()
    {
        await AsyncUnloadScene(_oldScene); // oldScene - должен быть к этому моменту отгружен, но как запасной план
        await AsyncUnloadScene(_currentScene);

        _ct?.Cancel();
        _ct?.Dispose();
    }

    public AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation sceneLocation, CancellationToken ct)
    {
        var handle = Addressables.LoadSceneAsync(sceneLocation, LoadSceneMode.Single, false);

        UpdateCurrentSceneAfterLoad(handle, ct).Forget();

        return handle;
    }

    private async UniTask AsyncUnloadScene(SceneInstance scene)
    {
        if (scene.Scene.IsValid() || scene.Scene.isLoaded)
            await Addressables.UnloadSceneAsync(scene).ToUniTask(cancellationToken: _ct.Token);
    }

    public async UniTask AsyncUnloadOldScene()
    {
        await AsyncUnloadScene(_oldScene);
    }

    private async UniTaskVoid UpdateCurrentSceneAfterLoad(AsyncOperationHandle<SceneInstance> handle, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        try
        {
            var result = await handle.ToUniTask(cancellationToken: linkedCTS.Token);

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _oldScene = _currentScene;
                _currentScene = result;
            }
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }
}
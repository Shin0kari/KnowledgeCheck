using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

public interface ISceneLoader
{
    // public AsyncOperation LoadSceneAsync(string sceneName);
    public AsyncOperationHandle<SceneInstance> LoadSceneAsync(IResourceLocation sceneLocation, CancellationToken ct);
    public UniTask AsyncUnloadOldScene();
}

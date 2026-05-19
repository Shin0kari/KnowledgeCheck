using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using static SceneUtils;

public interface IResourceLocationProvider
{
    public UniTask<IResourceLocation> GetSceneResourceLocation(SceneNames sceneName, CancellationToken ct);
    public UniTask<IResourceLocation> AsyncGetUploadResourceLocation(string gameobjectName, CancellationToken ct);
}
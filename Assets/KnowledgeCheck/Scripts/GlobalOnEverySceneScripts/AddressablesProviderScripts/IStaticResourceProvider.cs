using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using static SceneUtils;

public interface IStaticResourceProvider
{
    public UniTask<IResourceLocation> GetSceneLocation(SceneNames sceneName, CancellationToken ct);
}
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;
using static SceneUtils;

public class ChoicedSceneLoader : IDisposable
{
    private LoadingScreenController _loadingScreenController;
    private IResourceLocationProvider _sceneResourceLocationProvider;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        LoadingScreenController loadingScreenController,
        IResourceLocationProvider sceneResourceLocationProvider)
    {
        _loadingScreenController = loadingScreenController;
        _sceneResourceLocationProvider = sceneResourceLocationProvider;
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();
    }

    public async UniTask ChangeScene(SceneNames sceneName)
    {
        try
        {
            var sceneResourceLocation = await _sceneResourceLocationProvider.GetSceneResourceLocation(sceneName, _ct.Token);
            await _loadingScreenController.AsyncChangeScene(sceneResourceLocation, _ct.Token);
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }
}
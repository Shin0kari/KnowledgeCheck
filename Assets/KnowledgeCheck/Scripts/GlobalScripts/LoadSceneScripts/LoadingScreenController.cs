using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

public class LoadingScreenController : IDisposable
{
    private const float TIME_DELAY = 0.1f;
    private ISceneLoader _sceneLoader;

    private bool isStartLoadAnimationOver = false;

    public event Action<float> OnProgressChanged;
    public event Action OnStartAnimation;
    public event Action OnEndAnimation;

    private readonly CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(ISceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();

        OnProgressChanged = null;
        OnStartAnimation = null;
        OnEndAnimation = null;
    }

    public async UniTask AsyncChangeScene(IResourceLocation sceneResourceLocation, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        try
        {
            isStartLoadAnimationOver = false;

            OnStartAnimation?.Invoke();

            await LoadSceneAsync(sceneResourceLocation, linkedCTS.Token);

            while (OnEndAnimation == null) await UniTask.WaitForSeconds(TIME_DELAY, cancellationToken: linkedCTS.Token);
            OnEndAnimation?.Invoke();
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    private async UniTask LoadSceneAsync(IResourceLocation sceneResourceLocation, CancellationToken ct)
    {
        try
        {
            var loadSceneOperation = _sceneLoader.LoadSceneAsync(sceneResourceLocation, ct);

            while (!loadSceneOperation.IsDone || !isStartLoadAnimationOver)
            {
                OnProgressChanged?.Invoke(loadSceneOperation.PercentComplete);
                await UniTask.WaitForSeconds(TIME_DELAY, cancellationToken: ct);
            }

            await loadSceneOperation.Result.ActivateAsync().ToUniTask(cancellationToken: ct);
            await _sceneLoader.AsyncUnloadOldScene();
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    public void OnStartLoadAnimationOver()
    {
        isStartLoadAnimationOver = true;
    }
}

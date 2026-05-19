using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Zenject;

public class ChangeScene : IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private ChoicedSceneLoader _sceneLoader;

    private DisposableBag _dB;
    private UniTaskCompletionSource _loadingScreenLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        ChoicedSceneLoader sceneLoader
    )
    {
        _assetProvider = assetProvider;
        _sceneLoader = sceneLoader;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        _loadingScreenLoadedSource.TrySetCanceled();
        _loadingScreenLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<LoadingScreenView>()
            .OfType<IBindingSingletonComponent, LoadingScreenView>()
            .Subscribe(loadingScreen =>
            {
                if (loadingScreen == null)
                    return;

                var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
                    loadingScreen.gameObject.GetCancellationTokenOnDestroy(),
                    _ct.Token
                );

                loadingScreen.IsReady
                    .Subscribe(isReady =>
                    {
                        if (isReady)
                            _loadingScreenLoadedSource.TrySetResult();
                    }).AddTo(linkedCTS.Token);
            })
            .AddTo(ref _dB);
    }

    public async UniTask LoadMenuScene()
    {
        await _loadingScreenLoadedSource.Task.AttachExternalCancellation(_ct.Token);
        _sceneLoader.ChangeScene(SceneUtils.SceneNames.MainMenuScene).Forget();
    }
}
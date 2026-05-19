using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Zenject;

public class CheckButtonAvailabilty : IDisposable
{
    private IAssetProviderGetter _assetProvider;

    private IGetGameData _gameData;

    private ContinueGameButton _continueButton;
    private ScrollNewGameButton _scrollNewGameButton;
    private LoadMenuButton _loadGameButton;

    private SaveChecker _saveChecker;

    private DisposableBag _dB;
    private UniTaskCompletionSource _continueGameButtonLoadedSource = new();
    private UniTaskCompletionSource _scrollNewGameButtonLoadedSource = new();
    private UniTaskCompletionSource _loadMenuButtonLoadedSource = new();

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        IGetGameData gameData,
        SaveChecker saveChecker)
    {
        _assetProvider = assetProvider;
        _gameData = gameData;
        _saveChecker = saveChecker;

        SubscribeOnUpdateObjects();

        _saveChecker.IsCountDataChanged += CheckContinueGameButton;
        _saveChecker.IsCountDataChanged += CheckNewSaveButton;
        _saveChecker.IsCountDataChanged += CheckLoadGameButton;
    }

    public void Dispose()
    {
        _continueGameButtonLoadedSource.TrySetCanceled();
        _continueGameButtonLoadedSource = null;

        _scrollNewGameButtonLoadedSource.TrySetCanceled();
        _scrollNewGameButtonLoadedSource = null;

        _loadMenuButtonLoadedSource.TrySetCanceled();
        _loadMenuButtonLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ContinueGameButton>()
            .OfType<IBindingSingletonComponent, ContinueGameButton>()
            .Subscribe(continueGameButton =>
            {
                if (continueGameButton == null)
                    return;
                _continueButton = continueGameButton;
                _continueGameButtonLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<ScrollNewGameButton>()
            .OfType<IBindingSingletonComponent, ScrollNewGameButton>()
            .Subscribe(scrollNewGameButton =>
            {
                if (scrollNewGameButton == null)
                    return;
                _scrollNewGameButton = scrollNewGameButton;
                _scrollNewGameButtonLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<LoadMenuButton>()
            .OfType<IBindingSingletonComponent, LoadMenuButton>()
            .Subscribe(loadGameButton =>
            {
                if (loadGameButton == null)
                    return;
                _loadGameButton = loadGameButton;
                _loadMenuButtonLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void CheckContinueGameButton()
    {
        AsyncCheckContinueGameButton().Forget();
    }

    private async UniTask AsyncCheckContinueGameButton()
    {
        await _continueGameButtonLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        if (_continueButton == null || _gameData == null)
            return;
        if (_gameData.GetCurrentGameData().saveData == null)
            _continueButton.HideButton();
    }

    private void CheckNewSaveButton()
    {
        AsyncCheckNewSaveButton().Forget();
    }

    private async UniTask AsyncCheckNewSaveButton()
    {
        await _scrollNewGameButtonLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        if (_scrollNewGameButton == null || _gameData == null)
            return;
        if (_gameData.GetAllGameDatas().Count > 3)
            _scrollNewGameButton.HideButton();
        else
            _scrollNewGameButton.RevealButton();
    }

    private void CheckLoadGameButton()
    {
        AsyncCheckLoadGameButton().Forget();
    }

    private async UniTask AsyncCheckLoadGameButton()
    {
        await _loadMenuButtonLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        if (_loadGameButton == null || _gameData == null)
            return;
        if (_gameData.GetAllGameDatas().Count < 1)
            _loadGameButton.DisableButton();
        else
            _loadGameButton.EnableButton();
    }
}
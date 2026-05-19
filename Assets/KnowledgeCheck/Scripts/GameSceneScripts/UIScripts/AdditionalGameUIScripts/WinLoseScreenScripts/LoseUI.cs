using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

public class LoseUI : IDisposable
{
    private const float MAX_FADE = 1f;

    private IAssetProviderGetter _assetProvider;

    private float _maxColorCanvasFade = 0.5f;

    private float _colorCanvasFadeDuration = 0.5f;
    private float _losePanelFadeDuration = 0.5f;

    private CanvasGroup _redLoseCanvas;
    private CanvasGroup _blueLoseCanvas;
    private CanvasGroup _losePanelCanvas;

    private DisposableBag _dB;

    private UniTaskCompletionSource _blueCanvasGroupLoadedSource = new();
    private UniTaskCompletionSource _redCanvasGroupLoadedSource = new();
    private UniTaskCompletionSource _loseCanvasGroupLoadedSource = new();

    private UniTaskCompletionSource _allCanvasGroupLoadedSource = new();

    private CancellationToken _redLoseWindowToken;
    private CancellationToken _blueLoseWindowToken;
    private CancellationToken _losePanelToken;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        SubscribeOnUpdateObjects();
        UploadAddressablesCanvas().Forget();
    }

    public void Dispose()
    {
        _blueCanvasGroupLoadedSource.TrySetCanceled();
        _blueCanvasGroupLoadedSource = null;

        _redCanvasGroupLoadedSource.TrySetCanceled();
        _redCanvasGroupLoadedSource = null;

        _loseCanvasGroupLoadedSource.TrySetCanceled();
        _loseCanvasGroupLoadedSource = null;

        _allCanvasGroupLoadedSource.TrySetCanceled();
        _allCanvasGroupLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<BluePanelLinker>()
            .OfType<IBindingSingletonComponent, BluePanelLinker>()
            .Subscribe(bluePanelLinker =>
            {
                if (bluePanelLinker == null)
                    return;
                _blueLoseCanvas = bluePanelLinker.LinkerObject.GetComponent<CanvasGroup>();
                _blueCanvasGroupLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<RedPanelLinker>()
            .OfType<IBindingSingletonComponent, RedPanelLinker>()
            .Subscribe(redPanelLinker =>
            {
                if (redPanelLinker == null)
                    return;
                _redLoseCanvas = redPanelLinker.LinkerObject.GetComponent<CanvasGroup>();
                _redCanvasGroupLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<LosePanelLinker>()
            .OfType<IBindingSingletonComponent, LosePanelLinker>()
            .Subscribe(losePanelLinker =>
            {
                if (losePanelLinker == null)
                    return;
                _losePanelCanvas = losePanelLinker.LinkerObject.GetComponent<CanvasGroup>();
                _loseCanvasGroupLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private async UniTaskVoid UploadAddressablesCanvas()
    {
        try
        {
            await _blueCanvasGroupLoadedSource.Task.AttachExternalCancellation(_ct.Token);
            await _redCanvasGroupLoadedSource.Task.AttachExternalCancellation(_ct.Token);
            await _loseCanvasGroupLoadedSource.Task.AttachExternalCancellation(_ct.Token);

            ConfigurePanels();

            _allCanvasGroupLoadedSource.TrySetResult();
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
        catch (System.Exception err)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, err);
        }
    }

    private void ConfigurePanels()
    {
        if (_redLoseCanvas != null)
        {
            _redLoseCanvas.blocksRaycasts = false;
            _redLoseCanvas.interactable = false;
            _redLoseWindowToken = _redLoseCanvas.gameObject.GetCancellationTokenOnDestroy();
        }
        if (_blueLoseCanvas != null)
        {
            _blueLoseCanvas.blocksRaycasts = false;
            _blueLoseCanvas.interactable = false;
            _blueLoseWindowToken = _blueLoseCanvas.gameObject.GetCancellationTokenOnDestroy();
        }
        if (_losePanelCanvas != null)
        {
            _losePanelCanvas.blocksRaycasts = false;
            _losePanelCanvas.interactable = false;
            _losePanelToken = _losePanelCanvas.gameObject.GetCancellationTokenOnDestroy();
        }
    }

    public async UniTask OnDeath()
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, _redLoseWindowToken);
        await _allCanvasGroupLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        if (_redLoseCanvas == null)
            return;

        _redLoseCanvas.blocksRaycasts = true;
        _redLoseCanvas.interactable = true;

        await _redLoseCanvas
            .DOFade(_maxColorCanvasFade, _colorCanvasFadeDuration)
            .OnComplete(FadeLosePanel)
            .ToUniTask(cancellationToken: linkedCTS.Token);
    }

    public async UniTask OnDrown()
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, _blueLoseWindowToken);
        await _allCanvasGroupLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        if (_blueLoseCanvas == null)
            return;

        _blueLoseCanvas.blocksRaycasts = true;
        _blueLoseCanvas.interactable = true;

        await _blueLoseCanvas
            .DOFade(_maxColorCanvasFade, _colorCanvasFadeDuration)
            .OnComplete(FadeLosePanel)
            .ToUniTask(cancellationToken: linkedCTS.Token);
    }

    private async void FadeLosePanel()
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, _losePanelToken);

        if (_losePanelCanvas == null)
            return;

        _losePanelCanvas.blocksRaycasts = true;
        _losePanelCanvas.interactable = true;

        await _losePanelCanvas
            .DOFade(MAX_FADE, _losePanelFadeDuration)
            .ToUniTask(cancellationToken: linkedCTS.Token);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

public class WinUI : IDisposable
{
    private const float MAX_FADE = 1f;

    private IAssetProviderGetter _assetProvider;
    private UIUtilsProvider _uiUtilsProvider;

    private ReactiveProperty<ScriptableObject> _reactiveUIUtilsSO;
    private UIUtilsSO _uiUtilsSO;

    private WinUIUtils _winUIUtils;

    private CanvasGroup _whiteWindow;
    private CanvasGroup _winWindow;

    private float _winWindowFadeDuration;
    private float _whitenWindowFadeDuration;

    private DisposableBag _dB;

    private UniTaskCompletionSource _uiUtilsLoadedSource = new();
    private UniTaskCompletionSource _winUIUtilsLoadedSource = new();
    private UniTaskCompletionSource _winCanvasGroupLoadedSource = new();
    private UniTaskCompletionSource _whiteCanvasGroupLoadedSource = new();

    private UniTaskCompletionSource _allCanvasGroupLoadedSource = new();

    private CancellationToken _winWindowToken;
    private CancellationToken _whiteWindowToken;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        UIUtilsProvider uiUtilsProvider
    )
    {
        _assetProvider = assetProvider;
        _uiUtilsProvider = uiUtilsProvider;

        AsyncLoadResource().Forget();
        SubscribeOnUpdateObjects();
        UploadAddressablesCanvas().Forget();
    }

    public void Dispose()
    {
        _uiUtilsLoadedSource.TrySetCanceled();
        _uiUtilsLoadedSource = null;

        _winUIUtilsLoadedSource.TrySetCanceled();
        _winUIUtilsLoadedSource = null;

        _winCanvasGroupLoadedSource.TrySetCanceled();
        _winCanvasGroupLoadedSource = null;

        _whiteCanvasGroupLoadedSource.TrySetCanceled();
        _whiteCanvasGroupLoadedSource = null;

        _allCanvasGroupLoadedSource.TrySetCanceled();
        _allCanvasGroupLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private async UniTask AsyncLoadResource()
    {
        _reactiveUIUtilsSO = await _uiUtilsProvider.TryGetDataSO(_ct.Token);

        _reactiveUIUtilsSO?
            .Subscribe(uiUtilsSO =>
            {
                if (uiUtilsSO == null)
                    return;

                SetSO(uiUtilsSO);
            })
            .AddTo(ref _dB);
    }

    private void SetSO(ScriptableObject uiUtilsSO)
    {
        if (uiUtilsSO is not UIUtilsSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _uiUtilsSO = so;
        _uiUtilsLoadedSource.TrySetResult();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<WinUIUtils>()
            .OfType<IBindingSingletonComponent, WinUIUtils>()
            .Subscribe(winUIUtils =>
            {
                if (winUIUtils == null)
                    return;
                _winUIUtils = winUIUtils;
                _winUIUtilsLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<WinPanelLinker>()
            .OfType<IBindingSingletonComponent, WinPanelLinker>()
            .Subscribe(winPanelLinker =>
            {
                if (winPanelLinker == null)
                    return;

                _winWindow = winPanelLinker.LinkerObject.GetComponent<CanvasGroup>();
                _winCanvasGroupLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<WhitePanelLinker>()
            .OfType<IBindingSingletonComponent, WhitePanelLinker>()
            .Subscribe(whitePanelLinker =>
            {
                if (whitePanelLinker == null)
                    return;

                _whiteWindow = whitePanelLinker.LinkerObject.GetComponent<CanvasGroup>();
                _whiteCanvasGroupLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private async UniTaskVoid UploadAddressablesCanvas()
    {
        try
        {
            await _uiUtilsLoadedSource.Task.AttachExternalCancellation(_ct.Token);
            await _winUIUtilsLoadedSource.Task.AttachExternalCancellation(_ct.Token);
            await _winCanvasGroupLoadedSource.Task.AttachExternalCancellation(_ct.Token);
            await _whiteCanvasGroupLoadedSource.Task.AttachExternalCancellation(_ct.Token);

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
        if (_winUIUtils != null)
        {
            _winWindowFadeDuration = _winUIUtils.GetWinUIFadeDuration();
            _whitenWindowFadeDuration = _winUIUtils.GetWhiteUIFadeDuration();
        }

        if (_winWindow != null)
        {
            _winWindowToken = _winWindow.gameObject.GetCancellationTokenOnDestroy();
            _winWindow.blocksRaycasts = false;
            _winWindow.interactable = false;
        }
        if (_whiteWindow != null)
        {
            _whiteWindowToken = _whiteWindow.gameObject.GetCancellationTokenOnDestroy();
            _whiteWindow.blocksRaycasts = false;
            _whiteWindow.interactable = false;
        }
    }

    public async UniTask Win()
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, _winWindowToken);
        await _allCanvasGroupLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);
        if (_winWindow == null)
            return;

        _winWindow.blocksRaycasts = true;
        _winWindow.interactable = true;

        await _winWindow
            .DOFade(MAX_FADE, _winWindowFadeDuration)
            .ToUniTask(cancellationToken: linkedCTS.Token);
    }

    public async UniTask ChangeFadeWhiteWindow()
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, _whiteWindowToken);
        await _allCanvasGroupLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        await _whiteWindow
            .DOFade(MAX_FADE, _whitenWindowFadeDuration)
            .SetEase(_uiUtilsSO.WhiteWindowAnimCurve)
            .ToUniTask(cancellationToken: linkedCTS.Token);
    }
}
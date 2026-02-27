using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class WinUI : IInitializable
{
    private const float MAX_FADE = 1f;

    private WinUIUtils _winUIUtils;

    private CanvasGroup _whiteWindow;
    private CanvasGroup _winWindow;

    private float _winWindowFadeDuration;
    private float _whitenWindowFadeDuration;

    private Ease _whiteWindowAnimCurve = Ease.InOutQuart;

    private CancellationToken _winWindowToken;
    private CancellationToken _whiteWindowToken;

    [Inject]
    private void Construct(
        CanvasGroup whiteWindow,
        CanvasGroup winWindow,
        Ease whiteWindowAnimCurve,
        WinUIUtils winUIUtils)
    {
        _whiteWindow = whiteWindow;
        _winWindow = winWindow;
        _whiteWindowAnimCurve = whiteWindowAnimCurve;

        _winUIUtils = winUIUtils;

    }

    public void Initialize()
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

    public void Win()
    {
        if (_winWindow == null)
            return;

        _winWindow.blocksRaycasts = true;
        _winWindow.interactable = true;

        _winWindow
            .DOFade(MAX_FADE, _winWindowFadeDuration)
            .WithCancellation(_winWindowToken);
    }

    public void ChangeFadeWhiteWindow()
    {
        _whiteWindow
            .DOFade(MAX_FADE, _whitenWindowFadeDuration)
            .SetEase(_whiteWindowAnimCurve)
            .WithCancellation(_whiteWindowToken);
    }
}
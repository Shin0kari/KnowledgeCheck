using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class LoseUI
{
    private const float MAX_FADE = 1f;
    private float _maxColorCanvasFade = 0.5f;

    private float _colorCanvasFadeDuration = 0.5f;
    private float _losePanelFadeDuration = 0.5f;

    private CanvasGroup _redLoseCanvas;
    private CanvasGroup _blueLoseCanvas;
    private CanvasGroup _losePanelCanvas;

    private CancellationToken _redLoseWindowToken;
    private CancellationToken _blueLoseWindowToken;
    private CancellationToken _losePanelToken;

    [Inject]
    private void Construct(
        float maxColorCanvasFade,
        float colorCanvasFadeDuration,
        float losePanelFadeDuration,
        CanvasGroup redLoseCanvas,
        CanvasGroup blueLoseCanvas,
        CanvasGroup losePanelCanvas
    )
    {
        _maxColorCanvasFade = maxColorCanvasFade;
        _colorCanvasFadeDuration = colorCanvasFadeDuration;
        _losePanelFadeDuration = losePanelFadeDuration;
        _redLoseCanvas = redLoseCanvas;
        _blueLoseCanvas = blueLoseCanvas;
        _losePanelCanvas = losePanelCanvas;

        if (_redLoseCanvas != null)
        {
            _redLoseCanvas.blocksRaycasts = false;
            _redLoseCanvas.interactable = false;
        }
        if (_blueLoseCanvas != null)
        {
            _blueLoseCanvas.blocksRaycasts = false;
            _blueLoseCanvas.interactable = false;
        }
        if (_losePanelCanvas != null)
        {
            _losePanelCanvas.blocksRaycasts = false;
            _losePanelCanvas.interactable = false;
        }

        _redLoseWindowToken = _redLoseCanvas.gameObject.GetCancellationTokenOnDestroy();
        _blueLoseWindowToken = _blueLoseCanvas.gameObject.GetCancellationTokenOnDestroy();
        _losePanelToken = _losePanelCanvas.gameObject.GetCancellationTokenOnDestroy();
    }

    public void OnDeath()
    {
        if (_redLoseCanvas == null)
            return;

        _redLoseCanvas.blocksRaycasts = true;
        _redLoseCanvas.interactable = true;

        _redLoseCanvas
            .DOFade(_maxColorCanvasFade, _colorCanvasFadeDuration)
            .OnComplete(FadeLosePanel)
            .WithCancellation(_redLoseWindowToken);
    }

    public void OnDrown()
    {
        if (_blueLoseCanvas == null)
            return;

        _blueLoseCanvas.blocksRaycasts = true;
        _blueLoseCanvas.interactable = true;

        _blueLoseCanvas
            .DOFade(_maxColorCanvasFade, _colorCanvasFadeDuration)
            .OnComplete(FadeLosePanel)
            .WithCancellation(_blueLoseWindowToken);
    }

    private void FadeLosePanel()
    {
        if (_losePanelCanvas == null)
            return;

        _losePanelCanvas.blocksRaycasts = true;
        _losePanelCanvas.interactable = true;

        _losePanelCanvas
            .DOFade(MAX_FADE, _losePanelFadeDuration)
            .WithCancellation(_losePanelToken);
    }
}
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CongratulationPanel : MonoBehaviour
{
    private const float MAX_FADE_VALUE = 1f;
    private const float MIN_FADE_VALUE = 0f;
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _offset = 50f;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform[] _textParts;

    private Sequence _animationSequence;
    private Vector2[] _startTextPartPos;

    public event Action OnShowCompleteText;

    private void Awake()
    {
        _startTextPartPos = new Vector2[_textParts.Length];
        for (int i = 0; i < _textParts.Length; i++)
        {
            _startTextPartPos[i] = _textParts[i].anchoredPosition;
        }

        SetAnimationSequence();
    }

    private void OnDestroy()
    {
        if (_animationSequence.IsPlaying())
            _animationSequence.Complete();
        _animationSequence.Kill();
    }

    private void SetAnimationSequence()
    {
        _animationSequence = DOTween.Sequence();
        _animationSequence.Pause();

        _animationSequence.Append(_canvasGroup.DOFade(MAX_FADE_VALUE, _animationDuration).From(MIN_FADE_VALUE));
        Vector2[] directions = { Vector2.left, Vector2.up, Vector2.down, Vector2.right };

        for (int i = 0; i < _textParts.Length; i++)
        {
            var currentDir = i switch
            {
                0 => directions[0],
                int value when value % 2 == 0 && value != _textParts.Length - 1 => directions[1],
                int value when value % 2 == 1 && value != _textParts.Length - 1 => directions[2],
                _ => directions[3],
            };

            _animationSequence.Join(
                _textParts[i].DOAnchorPos(_startTextPartPos[i], _animationDuration)
                    .From(_startTextPartPos[i] + currentDir * _offset)
            );

        }
        _animationSequence.AppendCallback(SendShowCompleteTextSignal);
        _animationSequence.Append(_canvasGroup.DOFade(MIN_FADE_VALUE, _animationDuration));
        _animationSequence.OnComplete(OnCompleteAnimation);
        _animationSequence.SetAutoKill(false);
    }

    private void SendShowCompleteTextSignal()
    {
        OnShowCompleteText?.Invoke();
    }

    private void OnEnable()
    {
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        _canvasGroup.gameObject.SetActive(true);
        _animationSequence.Restart();
    }

    private void OnCompleteAnimation()
    {
        _canvasGroup.gameObject.SetActive(false);
    }
}
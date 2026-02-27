using System;
using DG.Tweening;
using UnityEngine;

public class BookAnimation : MonoBehaviour, IDisposable
{
    private const float MAX_FADE_VALUE = 1f;
    private const float MIN_FADE_VALUE = 0f;

    [SerializeField] private CanvasGroup _canvasGroup;

    private Tweener _bookCanvasFadeTween;

    [SerializeField] private float _yPosOffset = 0.5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _upAnimationTime = 3f;
    [SerializeField] private float _downAnimationTime = 3f;

    private RotateAnimationState _currentState;
    private GameObject _targetObject;
    private Vector3 _upTarget;

    private float _maxY;
    private float _minY;

    private Tweener _rotationTween;
    private Tweener _upperTween;
    private Tweener _downTween;

    private void Awake()
    {
        _maxY = transformHandle.position.y + _yPosOffset;
        _minY = transformHandle.position.y;

        // _upTarget = new(transformHandle.position.x, _maxY + 1f, transformHandle.position.z);
    }

    public void SetNewTargetObject(GameObject newTargetObject)
    {
        if (newTargetObject == null)
        {
            _targetObject = newTargetObject;
            DownAnimation();
        }
        else if (newTargetObject != null && _targetObject == null)
        {
            _targetObject = newTargetObject;
            UpAnimation();
        }
        else
            _targetObject = newTargetObject;
    }

    public void UpAnimation()
    {
        if (_targetObject == null)
            return;

        KillAllTweeners();

        _currentState = RotateAnimationState.Start;

        // _rotationTween = transform
        //     .DOLookAt(_targetObject.transform.position, _upAnimationTime)
        //     .SetEase(Ease.InOutQuint);

        _upperTween = transform
            .DOMoveY(_maxY, _upAnimationTime)
            .SetEase(Ease.OutElastic);
        OnBookCanvas();
    }

    public void DownAnimation()
    {
        KillAllTweeners();

        _currentState = RotateAnimationState.Final;
        _upTarget = new(transformHandle.position.x, _maxY + 1f, transformHandle.position.z);

        Vector3 _newRotationVector = new(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        _rotationTween = transform
            .DORotate(_newRotationVector, _downAnimationTime)
            //.DOLookAt(_upTarget, _downAnimationTime, AxisConstraint.None)
            .SetEase(Ease.OutBounce);
        _downTween = transform.DOMoveY(_minY, _downAnimationTime).SetEase(Ease.OutBounce);

        OffBookCanvas();
    }

    public void OnBookCanvas()
    {
        _bookCanvasFadeTween = _canvasGroup.DOFade(MAX_FADE_VALUE, _upAnimationTime);
    }

    public void OffBookCanvas()
    {
        _bookCanvasFadeTween = _canvasGroup.DOFade(MIN_FADE_VALUE, _downAnimationTime);
    }

    public void OffBookAnimation()
    {
        _currentState = RotateAnimationState.Final;

        OffBookCanvas();
    }

    private void Update()
    {
        if (_targetObject == null)
            return;

        if (_currentState == RotateAnimationState.Final)
            return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(_targetObject.transform.position - transform.position),
            Time.deltaTime * _rotationSpeed
        );
    }

    public void Dispose()
    {
        KillAllTweeners();
    }

    private void KillAllTweeners()
    {
        _rotationTween?.Kill();
        _upperTween?.Kill();
        _downTween?.Kill();
        _bookCanvasFadeTween?.Kill();
    }
}

public enum RotateAnimationState
{
    Start,
    Following,
    Final
}
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HoldMiniGameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject _rotatedObject;
    [SerializeField] private float _rotationSpeedMultiplier = 1f;
    private Coroutine _rotationCoroutine;
    private bool _isCursorOver;

    private float _currentRotation;
    private float _lastAngle;
    private float _limitAngleValue = -(RotationUtils.MAX_ROTATION - RotationUtils.START_ROTATION_VALUE);

    private float _currentAngle;
    private float _delta;
    private float _rotationStep;
    private float _finalAdjustment;

    private Vector2 _direction;

    private Mouse _currentMouse;

    public event Action OnCompleteMiniGame;

    private void Awake()
    {
        _currentMouse = Mouse.current;
    }

    private void OnDestroy()
    {
        OnCompleteMiniGame = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rotationCoroutine ??= StartCoroutine(RotateWhileHolding());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
            _rotationCoroutine = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isCursorOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isCursorOver = false;
    }

    private IEnumerator RotateWhileHolding()
    {
        while (true)
        {
            if (_isCursorOver)
                TryRotateToCursor();

            yield return null;
        }
    }

    private void OnEnable()
    {
        _currentRotation = RotationUtils.START_ROTATION_VALUE;
        _lastAngle = RotationUtils.START_ROTATION_VALUE;
    }

    private void TryRotateToCursor()
    {
        _currentAngle = GetAngleToCursor();

        _delta = Mathf.DeltaAngle(_currentAngle, _lastAngle);

        _rotationStep = _delta * _rotationSpeedMultiplier * Time.deltaTime;

        if (_delta > 0)
        {
            if (_currentRotation - _rotationStep >= _limitAngleValue)
            {
                _rotatedObject.transform.Rotate(0f, 0f, -_rotationStep);

                _currentRotation -= _rotationStep;
                _lastAngle = Mathf.MoveTowardsAngle(_lastAngle, _currentAngle, Mathf.Abs(_rotationStep));
            }
            else
            {
                _finalAdjustment = _currentRotation - _limitAngleValue;
                _rotatedObject.transform.Rotate(0f, 0f, -_finalAdjustment);

                _currentRotation = _limitAngleValue;

                OnCompleteMiniGame?.Invoke();
            }
        }
    }

    private float GetAngleToCursor()
    {
        _direction = _currentMouse.position.ReadValue();
        _direction.x -= _rotatedObject.transform.position.x;
        _direction.y -= _rotatedObject.transform.position.y;

        return Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
    }
}
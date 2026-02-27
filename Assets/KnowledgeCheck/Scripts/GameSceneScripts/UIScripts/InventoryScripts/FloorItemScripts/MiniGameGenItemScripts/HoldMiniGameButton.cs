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

    public event Action OnCompleteMiniGame;


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
        float currentAngle = GetAngleToCursor();

        float delta = Mathf.DeltaAngle(currentAngle, _lastAngle);

        float rotationStep = delta * _rotationSpeedMultiplier * Time.deltaTime;

        if (delta > 0)
        {
            if (_currentRotation - rotationStep >= _limitAngleValue)
            {
                _rotatedObject.transform.Rotate(0f, 0f, -rotationStep);

                _currentRotation -= rotationStep;
                _lastAngle = Mathf.MoveTowardsAngle(_lastAngle, currentAngle, Mathf.Abs(rotationStep));
            }
            else
            {
                float finalAdjustment = _currentRotation - _limitAngleValue;
                _rotatedObject.transform.Rotate(0f, 0f, -finalAdjustment);

                _currentRotation = _limitAngleValue;

                OnCompleteMiniGame?.Invoke();
            }
        }
    }

    // private void OldTryRotateToCursor()
    // {
    //     float currentAngle = GetAngleToCursor();

    //     float delta = Mathf.DeltaAngle(currentAngle, _lastAngle);

    //     if (delta > 0)
    //     {
    //         if (_currentRotation - delta >= _limitAngleValue)
    //         {
    //             _rotatedObject.transform.Rotate(0f, 0f, -delta);

    //             _currentRotation -= delta;
    //             _lastAngle = currentAngle;
    //         }
    //         else
    //         {
    //             _rotatedObject.transform.rotation.eulerAngles.Set(0f, 0f, _limitAngleValue);

    //             OnCompleteMiniGame?.Invoke();
    //         }
    //     }
    // }

    private float GetAngleToCursor()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 direction = mousePos - new Vector2(_rotatedObject.transform.position.x, _rotatedObject.transform.position.y);

        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(IGetMovementSpeed)), RequireComponent(typeof(IGetRotationSpeed))]
public class MovementAndRotationObserver : MonoBehaviour
{
    private Vector3 _characterPosition;
    private Vector3 _characterRotation;
    private Vector3 _oldCharacterPosition;
    private Vector3 _oldCharacterRotation;
    [SerializeField] private CharacterAnimation _characterAnimation;
    private IGetMovementSpeed _movementSpeedSender;
    private IGetRotationSpeed _rotationSpeedSender;
    private float _movementSpeedMultiplier;
    private float _rotationSpeedMultiplier;
    private float _maxAbsMovementSpeed;
    private float _absMaxYRotation;

    [Inject]
    private void Construct()
    {

        _movementSpeedSender = GetComponent<IGetMovementSpeed>();
        _rotationSpeedSender = GetComponent<IGetRotationSpeed>();

        _oldCharacterPosition = transform.position;
        _oldCharacterRotation = transform.rotation.eulerAngles;

        _absMaxYRotation = CharacterTurnUtils.BASE_DEG_PER_SEC * CharacterTurnUtils.MAX_ROTATION_VALUE * Time.fixedDeltaTime;
    }

    private void Start()
    {
        _movementSpeedMultiplier = _movementSpeedSender.GetMovementSpeed();
        _rotationSpeedMultiplier = _rotationSpeedSender.GetRotationSpeed() / CharacterTurnUtils.BASE_DEG_PER_SEC;
        _maxAbsMovementSpeed = _movementSpeedMultiplier * Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        Move();
        Look();
    }

    private bool _isStraightMoveStarted = false;
    private bool _isStrafeMoveStarted = false;
    private bool _isRotationStarted = false;
    private Vector3 _globalMovementOffset;
    private Vector3 _localMovementOffset;
    private Vector2 _movementOffset;
    private float _yRotationOffset;

    private float _rotationValue = 0f;
    private Vector2 _targetMovementValues = Vector2.zero;
    private Vector2 _currentMovementValues = Vector2.zero;

    private StraightDir _straightDir = StraightDir.idle;
    private StrafeDir _strafeDir = StrafeDir.idle;

    private Vector2 _moveDirection;
    private Vector2 _oldMoveDirection;

    private void Move()
    {
        _characterPosition = transform.position;

        if (_characterPosition.x != _oldCharacterPosition.x)
            _isStrafeMoveStarted = true;
        if (_characterPosition.z != _oldCharacterPosition.z)
            _isStraightMoveStarted = true;
        if (!_isStraightMoveStarted && !_isStrafeMoveStarted)
            return;

        _movementOffset = CalculateMovementOffset(_characterPosition, _oldCharacterPosition);
        _oldCharacterPosition = _characterPosition;

        CalculateAndUpdateCurrentMovementValues(
            ref _currentMovementValues,
            _movementOffset
        );

        if (Mathf.Abs(_currentMovementValues.x) < GeneralCharacterUtils.MEASUREMENT_ERROR)
        {
            _isStrafeMoveStarted = false;
            _currentMovementValues.x = 0f;
        }
        if (Mathf.Abs(_currentMovementValues.y) < GeneralCharacterUtils.MEASUREMENT_ERROR)
        {
            _isStraightMoveStarted = false;
            _currentMovementValues.y = 0f;
        }

        _characterAnimation.SetMoveAnimValue(_currentMovementValues);
    }

    private void CalculateAndUpdateCurrentMovementValues(
        ref Vector2 _currentMovementValues,
        in Vector2 _movementOffset
    )
    {
        _moveDirection = new(SignDirection(_movementOffset.x), SignDirection(_movementOffset.y));
        // Strafe move
        if (_moveDirection.x != _oldMoveDirection.x)
        {
            _oldMoveDirection.x = _moveDirection.x;
            _strafeDir = (StrafeDir)_moveDirection.x;
        }
        // Straight move
        if (_moveDirection.y != _oldMoveDirection.y)
        {
            _oldMoveDirection.y = _moveDirection.y;
            _straightDir = (StraightDir)_moveDirection.y;
        }

        _targetMovementValues = _movementOffset / _maxAbsMovementSpeed;
        _targetMovementValues.x = Mathf.Clamp(_targetMovementValues.x, -MAX_MOVEMENT_VALUE, MAX_MOVEMENT_VALUE);
        _targetMovementValues.y = Mathf.Clamp(_targetMovementValues.y, -MAX_MOVEMENT_VALUE, MAX_MOVEMENT_VALUE);

        UpdateCurrentMovementValues(
            ref _currentMovementValues,
            _targetMovementValues,
            _moveDirection,
            _straightDir,
            _strafeDir
        );
    }

    private Vector2 CalculateMovementOffset(
        Vector3 _characterPosition,
        Vector3 _oldCharacterPosition
    )
    {
        _globalMovementOffset = _characterPosition - _oldCharacterPosition;
        _localMovementOffset = transform.InverseTransformDirection(_globalMovementOffset);
        return new(_localMovementOffset.x, _localMovementOffset.z);
    }

    private const float MAX_MOVEMENT_VALUE = 1f;
    private const float ZERO_MOVEMENT_VALUE = 0f;

    private static void UpdateCurrentMovementValues(
        ref Vector2 _currentMovementValues,
        in Vector2 _targetMovementValues,
        in Vector2 _moveDirection,
        in StraightDir _straightDir,
        in StrafeDir _strafeDir
    )
    {
        switch (_straightDir)
        {
            case StraightDir.forward:
                if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.y) > GeneralCharacterUtils.MEASUREMENT_ERROR)
                {
                    _currentMovementValues.y += 2f * (_targetMovementValues.y - _currentMovementValues.y) * _moveDirection.y * Time.fixedDeltaTime;
                }
                else
                    _currentMovementValues.y = MAX_MOVEMENT_VALUE * _moveDirection.y;
                break;
            case StraightDir.backward:
                if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.y) > GeneralCharacterUtils.MEASUREMENT_ERROR)
                {
                    _currentMovementValues.y += 2f * (_targetMovementValues.y - _currentMovementValues.y) * _moveDirection.y * Time.fixedDeltaTime;
                }
                else
                    _currentMovementValues.y = MAX_MOVEMENT_VALUE * _moveDirection.y;
                break;
            case StraightDir.idle:
                if (Mathf.Abs(_currentMovementValues.y) > GeneralCharacterUtils.MEASUREMENT_ERROR)
                {
                    _currentMovementValues.y += 2f * (_targetMovementValues.y - _currentMovementValues.y) * Mathf.Sign(_moveDirection.y) * Time.fixedDeltaTime;
                }
                else
                    _currentMovementValues.y = ZERO_MOVEMENT_VALUE;
                break;
        }
        switch (_strafeDir)
        {
            case StrafeDir.left:
                if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.x) > GeneralCharacterUtils.MEASUREMENT_ERROR || Mathf.Sign(_currentMovementValues.x) > 0)
                {
                    _currentMovementValues.x += 2f * (_targetMovementValues.x - _currentMovementValues.x) * _moveDirection.x * Time.fixedDeltaTime;
                }
                else
                    _currentMovementValues.x = MAX_MOVEMENT_VALUE * _moveDirection.x;
                break;
            case StrafeDir.right:
                if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.x) > GeneralCharacterUtils.MEASUREMENT_ERROR || Mathf.Sign(_currentMovementValues.x) < 0)
                {
                    _currentMovementValues.x += 2f * (_targetMovementValues.x - _currentMovementValues.x) * _moveDirection.x * Time.fixedDeltaTime;
                }
                else
                    _currentMovementValues.x = MAX_MOVEMENT_VALUE * _moveDirection.x;
                break;
            case StrafeDir.idle:
                if (Mathf.Abs(_currentMovementValues.x) > GeneralCharacterUtils.MEASUREMENT_ERROR)
                {
                    _currentMovementValues.x += 2f * (_targetMovementValues.x - _currentMovementValues.x) * Mathf.Sign(_moveDirection.x) * Time.fixedDeltaTime;
                }
                else
                    _currentMovementValues.x = ZERO_MOVEMENT_VALUE;
                break;
        }
        _currentMovementValues.x = Mathf.Clamp(_currentMovementValues.x, -MAX_MOVEMENT_VALUE, MAX_MOVEMENT_VALUE);
        _currentMovementValues.y = Mathf.Clamp(_currentMovementValues.y, -MAX_MOVEMENT_VALUE, MAX_MOVEMENT_VALUE);
    }

    private static float SignDirection(float offset)
    {
        if (Mathf.Abs(offset) < GeneralCharacterUtils.MEASUREMENT_ERROR)
            return 0f;
        else
            return Mathf.Sign(offset);
    }

    private float _calculatedNewRotationOffset;

    private void Look()
    {
        _characterRotation = transform.rotation.eulerAngles;
        if (_characterRotation.y != _oldCharacterRotation.y)
        {
            _isRotationStarted = true;
        }

        if (!_isRotationStarted)
            return;

        _yRotationOffset = _characterRotation.y - _oldCharacterRotation.y;
        _oldCharacterRotation = _characterRotation;

        CalculateAndUpdateCurrentRotationValue(
            ref _rotationValue,
            _yRotationOffset,
            _rotationSpeedMultiplier
        );

        _characterAnimation.SetRotationAnimValue(_rotationValue);

        if (Mathf.Abs(_rotationValue) < GeneralCharacterUtils.MEASUREMENT_ERROR)
        {
            _isRotationStarted = false;
        }
    }

    private void CalculateAndUpdateCurrentRotationValue(
        ref float _rotationValue,
        in float _yRotationOffset,
        in float _rotationSpeedMultiplier
    )
    {
        _calculatedNewRotationOffset =
            CharacterTurnUtils.CalculateNewRotationYOffsetFromYOffset(
                _yRotationOffset,
                _rotationSpeedMultiplier
            );

        _rotationValue = _calculatedNewRotationOffset / _absMaxYRotation;
        _rotationValue = Mathf.Clamp(_rotationValue, -CharacterTurnUtils.MAX_ROTATION_VALUE, CharacterTurnUtils.MAX_ROTATION_VALUE);

        if (Mathf.Abs(_rotationValue) < GeneralCharacterUtils.MEASUREMENT_ERROR)
        {
            _rotationValue = 0f;
        }
    }
}
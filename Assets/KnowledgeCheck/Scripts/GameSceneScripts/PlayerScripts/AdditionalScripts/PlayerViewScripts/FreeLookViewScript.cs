using System;
using UnityEngine;
using Zenject;

public class FreeLookViewScript : AbstractViewScript
{
    const float DEAD_ZONE_MOVEMENT = 0.1f;

    [SerializeField] private float _movementSpeed;

    [SerializeField] private CharacterAnimation _characterAnimation;

    [SerializeField] private float _accelRotationTime = 0.2f;
    [SerializeField] private float _decelRotationTime = 0.2f;

    private bool _isStarted = true;

    private Vector2 _moveDirection;

    private CameraTrigger _cameraController;
    private CameraUtils _cameraUtils;

    private PlayerInputSystem _playerInput;

    private bool _isAccelStarightStarted = false;
    private bool _isAccelStrafeStarted = false;

    private Vector2 _oldMoveDirection = new();
    private float _translateStrafeTimer = 0f;
    private float _translateStraightTimer = 0f;

    private Vector2 _currentMovementValues = Vector2.zero;
    private Vector2 _startTranslationValues = Vector2.zero;

    private float _maxTimeAccelMovement = 0.2f;
    private float _maxTimeDecelMovement = 0.2f;

    private CurveType _accelTranslationCurveType = CurveType.InOut;
    private CurveType _decelTranslationCurveType = CurveType.InvertedIn;

    private StraightDir _straightDir = StraightDir.idle;

    private StrafeDir _strafeDir = StrafeDir.idle;
    private CurveAnimationUtils _curveAnimationUtils;
    private CurveType _accelRotationCurveType = CurveType.InOut;
    private CurveType _decelRotationCurveType = CurveType.InvertedIn;

    private ViewScriptUtils _viewUtils;

    private float _rotationTimer;

    private float _absMaxYRotation;
    private float _absYRotationOffset = 0f;
    private float _absYStartToCurrentRotation = 0f;

    private float _absEndAccelYRotation = 0f;
    private float _absStartDecelYRotation = 0f;

    private Vector3 _targetRotation;
    private Vector3 _currentRotation;
    private Vector3 _startRotation;

    private float _rotationYDir = 0f;
    private float _oldRotationYDir = 0f;

    private Vector3 _rotationOffset;
    private Vector3 _calculatedNewRotationOffset;
    private float _rotationSpeed = 0f;

    private bool _isStartedRotate = false; // показывает, начался ли поворот
    private bool _isRotateSpeedAccelerated = false; // показывает, началось ли ускорение поворота
    private bool _isRotateSpeedDecelerate = false; // показывает, началось ли замедление поворота

    [Inject]
    private void Construct(
        CameraTrigger cameraController,
        CameraUtils cameraUtils,
        CurveAnimationUtils curveAnimationUtils,
        ViewScriptUtils viewUtils)
    {
        _cameraController = cameraController;
        _cameraUtils = cameraUtils;
        _curveAnimationUtils = curveAnimationUtils;
        _viewUtils = viewUtils;

        _playerInput = new PlayerInputSystem();

        _targetRotation = transform.rotation.eulerAngles;
        _absMaxYRotation = CharacterTurnUtils.BASE_DEG_PER_SEC * CharacterTurnUtils.MAX_ROTATION_VALUE * Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        if (Enabled)
        {
            _moveDirection = _playerInput.Player.Move.ReadValue<Vector2>();
        }
        else
        {
            _moveDirection = new();
        }

        Move();
        Look();
    }

    private void OnEnable()
    {
        _playerInput.Enable();

        UpdateMovementValues();
        UpdateRotationValues();

        _isStarted = true;

        _cameraController.SetCameraTrigger(CameraTypes.FreeLookView);
    }

    private void OnDisable()
    {
        SetMovementValues();
        SetRotationValues();

        _isStarted = false;

        _playerInput.Disable();
    }

    private void UpdateMovementValues()
    {
        var movementValuesStruct = _viewUtils.GetMovementValues();
        _oldMoveDirection = movementValuesStruct.oldMoveDirection;
        _currentMovementValues = movementValuesStruct.currentMovementValues;
    }

    private void UpdateRotationValues()
    {
        var rotationValues = _viewUtils.GetRotationValues();
        _targetRotation = AngleNormalizer.GetNormalizedOffset(transform.rotation.eulerAngles) + new Vector3(0f, rotationValues.y, 0f);
    }

    private void SetMovementValues()
    {
        _viewUtils.SetMovementValues(
            new MovementValuesStruct()
            {
                currentMovementValues = _currentMovementValues,
                oldMoveDirection = _oldMoveDirection
            }
        );
    }

    private void SetRotationValues()
    {
        _viewUtils.SetRotationValues(_calculatedNewRotationOffset);
    }

    public override void Move()
    {
        // Strafe move
        if (_moveDirection.x != _oldMoveDirection.x || _isStarted)
        {
            _oldMoveDirection.x = _moveDirection.x;
            _strafeDir = (StrafeDir)CharacterMoveUtils.SignDirection(_oldMoveDirection.x);
            _startTranslationValues.x = _currentMovementValues.x;
            TimerScript.SetNewTimer(ref _translateStrafeTimer);
            _isAccelStrafeStarted = true;
        }
        // Straight move
        if (_moveDirection.y != _oldMoveDirection.y || _isStarted)
        {
            _oldMoveDirection.y = _moveDirection.y;
            _straightDir = (StraightDir)CharacterMoveUtils.SignDirection(_oldMoveDirection.y);
            _startTranslationValues.y = _currentMovementValues.y;
            TimerScript.SetNewTimer(ref _translateStraightTimer);
            _isAccelStarightStarted = true;
        }

        _isStarted = false;

        if (!_isAccelStarightStarted && !_isAccelStrafeStarted)
            return;

        CharacterMoveUtils.UpdateCurrentMovementValues(
            ref _currentMovementValues,
            ref _isAccelStarightStarted,
            ref _isAccelStrafeStarted,
            _straightDir,
            _strafeDir,
            _oldMoveDirection,
            _curveAnimationUtils,
            _accelTranslationCurveType,
            _decelTranslationCurveType,
            _startTranslationValues,
            _maxTimeAccelMovement,
            _maxTimeDecelMovement,
            ref _translateStraightTimer,
            ref _translateStrafeTimer
        );

        _characterAnimation.SetMoveAnimValue(_currentMovementValues);

        float scaledMovementSpeed = _movementSpeed * Time.fixedDeltaTime;
        Vector3 offset = new Vector3(_currentMovementValues.x, 0f, _currentMovementValues.y) * scaledMovementSpeed;

        transform.Translate(offset);
    }

    public override void Look()
    {
        _currentRotation = AngleNormalizer.GetNormalizedOffset(transform.rotation.eulerAngles);
        // Всегда считаем новый TargetRotation
        if (_moveDirection.sqrMagnitude > DEAD_ZONE_MOVEMENT)
            SetTargetRotation();

        // Проверяем нужно ли поворачивать объект
        if (!CheckRotationMatch(_targetRotation, _currentRotation))
        {
            if (_isStartedRotate)
            {
                _isStartedRotate = false;
                _oldRotationYDir = 0f;

                _rotationSpeed = 0f;
                _characterAnimation.SetRotationAnimValue(_rotationSpeed);
            }
            return;
        }
        else
        {
            _rotationOffset = AngleNormalizer.GetNormalizedOffset(_targetRotation - _currentRotation);

            _rotationYDir = MathF.Sign(_rotationOffset.y);
        }

        _calculatedNewRotationOffset = CharacterTurnUtils.CalculateFreeLookNewRotationOffset(
            _curveAnimationUtils,
            _accelRotationCurveType,
            _decelRotationCurveType,
            _rotationYDir,
            ref _oldRotationYDir,
            ref _isStartedRotate,
            ref _isRotateSpeedAccelerated,
            ref _isRotateSpeedDecelerate,
            _rotationOffset,
            ref _startRotation,
            _currentRotation,
            _targetRotation,
            ref _absEndAccelYRotation,
            ref _absStartDecelYRotation,
            _accelRotationTime,
            _decelRotationTime,
            ref _rotationTimer
        );

        // Считаем скорость анимации и устанавливаем её
        _rotationSpeed = Mathf.Clamp(_calculatedNewRotationOffset.y, -_absMaxYRotation, _absMaxYRotation) / _absMaxYRotation;

        _characterAnimation.SetRotationAnimValue(_rotationSpeed);

        // Поворачиваем персонажа относительно сдвига
        transform.Rotate(_calculatedNewRotationOffset);

        // Обновляем значения всеъ зон поворота
        UpdateZoneData();
    }

    private void UpdateZoneData()
    {
        float anglePassed = _absYStartToCurrentRotation;
        float remainingAngle = _absYRotationOffset;

        // Если в данный момент состояние ускорения поворота
        if (anglePassed <= _absEndAccelYRotation && !_isRotateSpeedAccelerated)
        {
            TimerScript.UpdateTimer(ref _rotationTimer);
        }
        // Если в данный момент состояние полной скорости
        else if (remainingAngle > _absStartDecelYRotation)
        {
            if (!_isRotateSpeedAccelerated) _isRotateSpeedAccelerated = true;
            if (_isRotateSpeedDecelerate)
            {
                _isStartedRotate = false;
                _isRotateSpeedAccelerated = false;
                _isRotateSpeedDecelerate = false;
            }
        }
        // Если в данный момент состояние замедления поворота
        else
        {
            if (!_isRotateSpeedAccelerated)
                _isRotateSpeedAccelerated = true;
            if (!_isRotateSpeedDecelerate)
                _isRotateSpeedDecelerate = true;

            TimerScript.UpdateTimer(ref _rotationTimer);
        }
    }

    private bool CheckRotationMatch(Vector3 newRotationOffset, Vector3 oldRotationOffset)
    {
        return
        ((newRotationOffset.x,
            newRotationOffset.y,
            newRotationOffset.z),
        (oldRotationOffset.x,
            oldRotationOffset.y,
            oldRotationOffset.z))
        switch
        {
            ((float nX, _, _), (float oX, _, _)) when Mathf.Abs(nX - oX) > GeneralCharacterUtils.MEASUREMENT_ERROR => true,
            ((_, float nY, _), (_, float oY, _)) when Mathf.Abs(nY - oY) > GeneralCharacterUtils.MEASUREMENT_ERROR => true,
            ((_, _, float nZ), (_, _, float oZ)) when Mathf.Abs(nZ - oZ) > GeneralCharacterUtils.MEASUREMENT_ERROR => true,
            _ => false,
        };
    }

    private void SetTargetRotation()
    {
        _targetRotation = AngleNormalizer.GetNormalizedOffset(_cameraUtils.gameObject.transform.rotation.eulerAngles);
        _targetRotation.x = 0f;
        _targetRotation.z = 0f;
    }
}
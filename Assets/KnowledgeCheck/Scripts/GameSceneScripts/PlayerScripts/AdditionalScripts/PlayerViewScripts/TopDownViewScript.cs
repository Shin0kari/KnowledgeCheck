using UnityEngine;
using Zenject;

public class TopDownViewScript : AbstractViewScript
{
    const float DEAD_ZONE_MOVEMENT = 0.1f;

    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _cameraRotateSpeed;
    [SerializeField] private float _accelerationRotateSpeed = 0.6f;
    [SerializeField] private CharacterAnimation _characterAnimation;

    private bool _isStarted = true;

    private PlayerInputSystem _playerInput;

    private Vector2 _moveDirection;
    private Vector2 _lookDirection;

    private CameraTrigger _cameraController;

    private CurveAnimationUtils _curveAnimationUtils;

    private bool _isAccelStarightStarted = false;
    private bool _isAccelStrafeStarted = false;

    private ViewScriptUtils _viewUtils;

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

    private const float BASE_DEG_PER_SEC = 90f;
    private const float MAX_ROTATION_VALUE = 1f;
    private float _absMaxYRotation;
    private float _rotationSpeed = 0f;
    private float _targetRotationSpeed = 0f;
    private float _rotationVelocity = 0.0f;
    private Vector2 _currentRotationValues = Vector2.zero;
    private Vector3 _calculatedNewRotationOffset;

    [Inject]
    private void Construct(
        CameraTrigger cameraController,
        CurveAnimationUtils curveAnimationUtils,
        ViewScriptUtils viewUtils)
    {
        _cameraController = cameraController;
        _curveAnimationUtils = curveAnimationUtils;
        _viewUtils = viewUtils;

        _playerInput = new PlayerInputSystem();

        _absMaxYRotation = BASE_DEG_PER_SEC * MAX_ROTATION_VALUE * Time.fixedDeltaTime;
    }

    private void OnEnable()
    {
        _playerInput.Enable();

        UpdateMovementValues();
        UpdateRotationValues();

        _isStarted = true;

        _cameraController.SetCameraTrigger(CameraTypes.TopDownView);
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
        _rotationSpeed = Mathf.Clamp(rotationValues.y, -_absMaxYRotation, _absMaxYRotation) / _absMaxYRotation;
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

    void FixedUpdate()
    {
        if (Enabled)
        {
            _moveDirection = _playerInput.Player.Move.ReadValue<Vector2>();
            _lookDirection = _playerInput.Player.Look.ReadValue<Vector2>();
        }
        else
        {
            _moveDirection = new();
            _lookDirection = new();
        }

        Move();
        Look();
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
        if (_lookDirection.sqrMagnitude < DEAD_ZONE_MOVEMENT)
        {
            if (_rotationSpeed == 0)
                return;
            if (Mathf.Abs(_rotationSpeed) < DEAD_ZONE_MOVEMENT)
            {
                _rotationSpeed = 0f;
                _characterAnimation.SetRotationAnimValue(_rotationSpeed);
            }
            else
            {
                _rotationSpeed = Mathf.SmoothDamp(
                    current: _rotationSpeed,
                    target: 0f,
                    currentVelocity: ref _rotationVelocity,
                    smoothTime: _accelerationRotateSpeed
                );
                _characterAnimation.SetRotationAnimValue(_rotationSpeed);
            }
            return;
        }

        _calculatedNewRotationOffset = CharacterTurnUtils.CalculateNewRotationOffsetFromLookDir(
            _lookDirection,
            _cameraRotateSpeed
        );

        _targetRotationSpeed = Mathf.Clamp(_calculatedNewRotationOffset.y, -_absMaxYRotation, _absMaxYRotation) / _absMaxYRotation;

        _rotationSpeed = Mathf.SmoothDamp(
            current: _rotationSpeed,
            target: _targetRotationSpeed,
            currentVelocity: ref _rotationVelocity,
            smoothTime: _accelerationRotateSpeed
        );

        _characterAnimation.SetRotationAnimValue(_rotationSpeed);

        transform.Rotate(_calculatedNewRotationOffset);
    }
}
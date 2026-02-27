using UnityEngine;

public static class CharacterTurnUtils
{
    public const float BASE_DEG_PER_SEC = 90f;
    public const float MAX_ROTATION_VALUE = 1f;

    private static float _absYRotationOffset;
    private static float _absYStartToCurrentRotation;
    private static float _endAccelYRotationOffset;
    private static Vector3 _calculatedNewRotationOffset;
    private static float _currentRotationOffset;

    public static Vector3 CalculateFreeLookNewRotationOffset(
        in CurveAnimationUtils _curveAnimationUtils,
        in CurveType _accelRotationCurveType,
        in CurveType _decelRotationCurveType,
        in float _rotationYDir,
        ref float _oldRotationYDir,
        ref bool _isStartedRotate,
        ref bool _isRotateSpeedAccelerated,
        ref bool _isRotateSpeedDecelerate,
        in Vector3 _rotationOffset,
        ref Vector3 _startRotation,
        in Vector3 _currentRotation,
        in Vector3 _targetRotation,
        ref float _absEndAccelYRotation,
        ref float _absStartDecelYRotation,
        in float _accelRotationTime,
        in float _decelRotationTime,
        ref float _rotationTimer
    )
    {
        if (_rotationYDir != _oldRotationYDir)
        {
            _isStartedRotate = false;
            _isRotateSpeedAccelerated = false;
            _oldRotationYDir = _rotationYDir;
        }

        _absYRotationOffset = Mathf.Abs(_rotationOffset.y);

        // До начала разгона задаём новый таймер и AccelAndDecelYRotationDirections
        // Иначе считаем новый DecelYRotationDirection
        if (!_isStartedRotate && !_isRotateSpeedAccelerated)
        {
            _startRotation = _currentRotation;

            SetAccelAndDecelYRotationDirections(
                _absYRotationOffset,
                _curveAnimationUtils,
                _accelRotationCurveType,
                _decelRotationCurveType,
                _accelRotationTime,
                _decelRotationTime,
                ref _absEndAccelYRotation,
                ref _absStartDecelYRotation
            );
            _endAccelYRotationOffset = AngleNormalizer.NormalizeAngle(_startRotation.y + _absEndAccelYRotation * _rotationYDir);
            // _startDecelYRotationOffset = AngleNormalizer.NormalizeAngle(_targetRotation.y - _absStartDecelYRotation * _rotationYDir);

            TimerScript.SetNewTimer(ref _rotationTimer);
            _isStartedRotate = true;
        }
        else
        {
            if (!_isRotateSpeedAccelerated && _absYStartToCurrentRotation <= _absEndAccelYRotation && _absYRotationOffset <= _absStartDecelYRotation)
            {
                _startRotation = _currentRotation;

                SetAccelAndDecelYRotationDirections(
                    _absYRotationOffset,
                    _curveAnimationUtils,
                    _accelRotationCurveType,
                    _decelRotationCurveType,
                    _accelRotationTime,
                    _decelRotationTime,
                    ref _absEndAccelYRotation,
                    ref _absStartDecelYRotation
                );
                _endAccelYRotationOffset = AngleNormalizer.NormalizeAngle(_startRotation.y + _absEndAccelYRotation * _rotationYDir);
                // _startDecelYRotationOffset = AngleNormalizer.NormalizeAngle(_targetRotation.y - _absStartDecelYRotation * _rotationYDir);

                TimerScript.SetNewTimer(ref _rotationTimer);
                _isStartedRotate = true;
            }
            else
            {
                SetDecelYRotationDirection(
                    _absYRotationOffset,
                    _curveAnimationUtils,
                    _decelRotationCurveType,
                    _decelRotationTime,
                    ref _absStartDecelYRotation
                );
                // _startDecelYRotationOffset = AngleNormalizer.NormalizeAngle(_targetRotation.y - _absStartDecelYRotation * _rotationYDir);

                if (_absYRotationOffset == _absStartDecelYRotation || (!_isRotateSpeedDecelerate && _absYRotationOffset < _absStartDecelYRotation))
                    TimerScript.SetNewTimer(ref _rotationTimer);
            }
        }

        _absYStartToCurrentRotation = Mathf.Abs(Mathf.DeltaAngle(_startRotation.y, _currentRotation.y));

        // Вычисление сдвига поворота персонажа
        _calculatedNewRotationOffset =
            AngleNormalizer.GetNormalizedOffset(
                CalculateNewRotationOffsetWithAnimationCurve(
                    _curveAnimationUtils,
                    _rotationYDir,
                    ref _isRotateSpeedAccelerated,
                    ref _startRotation,
                    _currentRotation,
                    _targetRotation,
                    ref _absEndAccelYRotation,
                    ref _absStartDecelYRotation,
                    _accelRotationTime,
                    _decelRotationTime,
                    ref _rotationTimer
                ));

        return _calculatedNewRotationOffset;
    }

    private static void SetAccelAndDecelYRotationDirections(
        in float absYOffset,
        in CurveAnimationUtils _curveAnimationUtils,
        in CurveType _accelRotationCurveType,
        in CurveType _decelRotationCurveType,
        in float _accelRotationTime,
        in float _decelRotationTime,
        ref float _absEndAccelYRotation,
        ref float _absStartDecelYRotation)
    {
        float accelRotationOffset = _curveAnimationUtils.GetAccuracyCurveAverage(_accelRotationCurveType) * BASE_DEG_PER_SEC * _accelRotationTime;
        float decelRotationOffset = _curveAnimationUtils.GetAccuracyCurveAverage(_decelRotationCurveType) * BASE_DEG_PER_SEC * _decelRotationTime;

        if (absYOffset < accelRotationOffset + decelRotationOffset)
        {
            _absEndAccelYRotation = absYOffset / 2;
            _absStartDecelYRotation = absYOffset / 2;
        }
        else
        {
            _absEndAccelYRotation = accelRotationOffset;
            _absStartDecelYRotation = decelRotationOffset;
        }
    }

    private static void SetDecelYRotationDirection(
        in float absYOffset,
        in CurveAnimationUtils _curveAnimationUtils,
        in CurveType _decelRotationCurveType,
        in float _decelRotationTime,
        ref float _absStartDecelYRotation)
    {
        float decelRotationOffset = _curveAnimationUtils.GetAccuracyCurveAverage(_decelRotationCurveType) * BASE_DEG_PER_SEC * _decelRotationTime;
        if (absYOffset < decelRotationOffset)
            _absStartDecelYRotation = absYOffset;
        else
            _absStartDecelYRotation = decelRotationOffset;
    }

    private static Vector3 CalculateNewRotationOffsetWithAnimationCurve(
        in CurveAnimationUtils _curveAnimationUtils,
        in float _rotationYDir,
        ref bool _isRotateSpeedAccelerated,
        ref Vector3 _startRotation,
        in Vector3 _currentRotation,
        in Vector3 _targetRotation,
        ref float _absEndAccelYRotation,
        ref float _absStartDecelYRotation,
        in float _accelRotationTime,
        in float _decelRotationTime,
        ref float _rotationTimer
    )
    {
        float anglePassed = _absYStartToCurrentRotation;
        float remainingAngle = _absYRotationOffset;
        float rotationYOffset;
        Vector3 newRotationOffset;
        // Vector3 newRotationOffset = _currentRotation;

        // Если в данный момент состояние ускорения поворота
        if (anglePassed <= _absEndAccelYRotation && !_isRotateSpeedAccelerated && remainingAngle > _absStartDecelYRotation)
        {
            // Причина, почему не _currentRotation, а _startRotation, в том, что в отличии от замедления поворота при новом повороте у игрока не 
            rotationYOffset =
                    Mathf.Clamp(Mathf.DeltaAngle(_startRotation.y, _endAccelYRotationOffset), -_absEndAccelYRotation, _absEndAccelYRotation) *
                    _curveAnimationUtils.GetAnimationCurveValue(CurveType.In, _rotationTimer, _accelRotationTime) *
                    Time.fixedDeltaTime;

            rotationYOffset *= 15.15f;
        }
        // Если в данный момент состояние полной скорости
        else if (remainingAngle > _absStartDecelYRotation)
        {
            rotationYOffset = _rotationYDir * BASE_DEG_PER_SEC * MAX_ROTATION_VALUE * Time.fixedDeltaTime;
        }
        // Если в данный момент состояние замедления поворота
        else
        {
            // Причина почему не _startDecelYRotationOffset а _currentRotation в том, что если игрок всё время будет поворачивать персонажа, 
            // то персонаж сможет в теории совершить поворот больше разрешённого
            rotationYOffset =
                Mathf.Clamp(Mathf.DeltaAngle(_currentRotation.y, _targetRotation.y), -_absStartDecelYRotation, _absStartDecelYRotation) *
                (MAX_ROTATION_VALUE - _curveAnimationUtils.GetAnimationCurveValue(CurveType.InvertedOut, _rotationTimer, _decelRotationTime)) *
                Time.fixedDeltaTime;

            if (Mathf.Abs(rotationYOffset) < GeneralCharacterUtils.MEASUREMENT_ERROR)
                rotationYOffset = remainingAngle * _rotationYDir;
            else
                rotationYOffset *= 21.27f;
        }

        newRotationOffset = new(0f, rotationYOffset, 0f);

        return newRotationOffset;
    }

    public static Vector3 CalculateNewRotationOffsetFromLookDir(
        Vector2 _lookDirection,
        float _cameraRotateSpeed
    )
    {
        _currentRotationOffset = CalculateNewRotationYOffsetFromYOffset(_lookDirection.x, _cameraRotateSpeed);
        _calculatedNewRotationOffset = new(0f, _currentRotationOffset, 0f);
        return _calculatedNewRotationOffset;
    }

    public static float CalculateNewRotationYOffsetFromYOffset(
        float yOffset,
        float _cameraRotateSpeed
    )
    {
        yOffset *= 2f * _cameraRotateSpeed * Time.fixedDeltaTime;
        yOffset = Mathf.Clamp(yOffset, -BASE_DEG_PER_SEC, BASE_DEG_PER_SEC);
        return yOffset;
    }
}
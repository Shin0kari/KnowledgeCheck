using UnityEngine;

public static class CharacterMoveUtils
{
    private const float MAX_MOVEMENT_VALUE = 1f;
    private const float ZERO_MOVEMENT_VALUE = 0f;

    public static void UpdateCurrentMovementValues(
        ref Vector2 _currentMovementValues,
        ref bool _isAccelStarightStarted,
        ref bool _isAccelStrafeStarted,
        in StraightDir _straightDir,
        in StrafeDir _strafeDir,
        in Vector2 _oldMoveDirection,
        in CurveAnimationUtils _curveAnimationUtils,
        in CurveType _accelTranslationCurveType,
        in CurveType _decelTranslationCurveType,
        in Vector2 _startTranslationValues,
        in float _maxTimeAccelMovement,
        in float _maxTimeDecelMovement,
        ref float _translateStraightTimer,
        ref float _translateStrafeTimer
    )
    {
        if (_isAccelStarightStarted)
        {
            switch (_straightDir)
            {
                case StraightDir.forward:
                    if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.y) > GeneralCharacterUtils.MEASUREMENT_ERROR || Mathf.Sign(_currentMovementValues.y) < 0)
                    {
                        // Ускоряемся вперёд
                        _currentMovementValues.y =
                            _curveAnimationUtils
                                .GetAnimationCurveValue(
                                    _accelTranslationCurveType,
                                    _startTranslationValues.y,
                                    _straightDir,
                                    ChangeValueType.Increase,
                                    _maxTimeAccelMovement - (int)_straightDir * _startTranslationValues.y * _maxTimeAccelMovement,
                                    _translateStraightTimer);
                    }
                    else
                    {
                        _currentMovementValues.y = MAX_MOVEMENT_VALUE * Mathf.Sign(_oldMoveDirection.y);
                    }
                    break;
                case StraightDir.backward:
                    if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.y) > GeneralCharacterUtils.MEASUREMENT_ERROR || Mathf.Sign(_currentMovementValues.y) > 0)
                    {
                        // Ускоряемся назад
                        _currentMovementValues.y =
                            _curveAnimationUtils
                                .GetAnimationCurveValue(
                                    _accelTranslationCurveType,
                                    _startTranslationValues.y,
                                    _straightDir,
                                    ChangeValueType.Increase,
                                    _maxTimeAccelMovement - (int)_straightDir * _startTranslationValues.y * _maxTimeAccelMovement,
                                    _translateStraightTimer);
                    }
                    else
                    {
                        _currentMovementValues.y = MAX_MOVEMENT_VALUE * Mathf.Sign(_oldMoveDirection.y);
                    }
                    break;
                case StraightDir.idle:
                    if (Mathf.Abs(_currentMovementValues.y) > GeneralCharacterUtils.MEASUREMENT_ERROR)
                    {
                        // Замедляемся
                        _currentMovementValues.y =
                            Mathf.Sign(_currentMovementValues.y) *
                            _curveAnimationUtils
                                .GetAnimationCurveAbsValue(
                                    _decelTranslationCurveType,
                                    Mathf.Abs(_startTranslationValues.y),
                                    ChangeValueType.Decrease,
                                    _maxTimeDecelMovement - _maxTimeDecelMovement * (MAX_MOVEMENT_VALUE - Mathf.Abs(_startTranslationValues.y)),
                                    _translateStraightTimer);
                    }
                    else
                    {
                        _currentMovementValues.y = ZERO_MOVEMENT_VALUE;
                        _isAccelStarightStarted = false;
                    }
                    break;
            }
            TimerScript.UpdateTimer(ref _translateStraightTimer);
        }

        if (_isAccelStrafeStarted)
        {
            switch (_strafeDir)
            {
                case StrafeDir.left:
                    if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.x) > GeneralCharacterUtils.MEASUREMENT_ERROR || Mathf.Sign(_currentMovementValues.x) > 0)
                    {
                        // Ускоряемся
                        _currentMovementValues.x =
                            _curveAnimationUtils
                                .GetAnimationCurveValue(
                                    _accelTranslationCurveType,
                                    _startTranslationValues.x,
                                    _strafeDir,
                                    ChangeValueType.Increase,
                                    _maxTimeAccelMovement - (int)_strafeDir * _startTranslationValues.x * _maxTimeAccelMovement,
                                    _translateStrafeTimer);
                    }
                    else
                    {
                        _currentMovementValues.x = MAX_MOVEMENT_VALUE * Mathf.Sign(_oldMoveDirection.x);
                    }
                    break;
                case StrafeDir.right:
                    if (MAX_MOVEMENT_VALUE - Mathf.Abs(_currentMovementValues.x) > GeneralCharacterUtils.MEASUREMENT_ERROR || Mathf.Sign(_currentMovementValues.x) < 0)
                    {
                        // Ускоряемся
                        _currentMovementValues.x =
                            _curveAnimationUtils
                                .GetAnimationCurveValue(
                                    _accelTranslationCurveType,
                                    _startTranslationValues.x,
                                    _strafeDir,
                                    ChangeValueType.Increase,
                                    _maxTimeAccelMovement - (int)_strafeDir * _startTranslationValues.x * _maxTimeAccelMovement,
                                    _translateStrafeTimer);
                    }
                    else
                    {
                        _currentMovementValues.x = MAX_MOVEMENT_VALUE * Mathf.Sign(_oldMoveDirection.x);
                    }
                    break;
                case StrafeDir.idle:
                    if (Mathf.Abs(_currentMovementValues.x) > GeneralCharacterUtils.MEASUREMENT_ERROR)
                    {
                        // Замедляемся
                        _currentMovementValues.x =
                            Mathf.Sign(_currentMovementValues.x) *
                            _curveAnimationUtils
                                .GetAnimationCurveAbsValue(
                                    _decelTranslationCurveType,
                                    Mathf.Abs(_startTranslationValues.x),
                                    ChangeValueType.Decrease,
                                    _maxTimeDecelMovement - _maxTimeDecelMovement * (MAX_MOVEMENT_VALUE - Mathf.Abs(_startTranslationValues.x)),
                                    _translateStrafeTimer);
                    }
                    else
                    {
                        _currentMovementValues.x = ZERO_MOVEMENT_VALUE;
                        _isAccelStrafeStarted = false;
                    }
                    break;
            }
            TimerScript.UpdateTimer(ref _translateStrafeTimer);
        }
    }

    public static float SignDirection(float offset)
    {
        if (Mathf.Abs(offset) < GeneralCharacterUtils.MEASUREMENT_ERROR)
            return 0f;
        else
            return Mathf.Sign(offset);
    }
}
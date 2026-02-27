using UnityEngine;

public class ViewScriptUtils
{
    private Vector2 _oldMoveDirection = Vector2.zero;
    private Vector2 _currentMovementValues = Vector2.zero;

    private Vector3 _rotationOffset;

    public MovementValuesStruct GetMovementValues()
    {
        return new MovementValuesStruct()
        {
            oldMoveDirection = _oldMoveDirection,
            currentMovementValues = _currentMovementValues
        };
    }
    public void SetMovementValues(MovementValuesStruct movementValuesStruct)
    {
        _oldMoveDirection = movementValuesStruct.oldMoveDirection;
        _currentMovementValues = movementValuesStruct.currentMovementValues;
    }

    public Vector3 GetRotationValues()
    {
        return _rotationOffset;
    }
    public void SetRotationValues(Vector3 newRotationOffset)
    {
        _rotationOffset = newRotationOffset;
    }
}

public struct MovementValuesStruct
{
    public Vector2 oldMoveDirection;
    public Vector2 currentMovementValues;
}
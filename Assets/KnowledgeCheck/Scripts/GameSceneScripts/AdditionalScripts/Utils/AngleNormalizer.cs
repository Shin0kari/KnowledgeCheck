using UnityEngine;

public static class AngleNormalizer
{
    public static Vector3 GetNormalizedOffset(Vector3 offset)
    {
        offset.x = NormalizeAngle(offset.x);
        offset.y = NormalizeAngle(offset.y);
        offset.z = NormalizeAngle(offset.z);

        return offset;
    }

    public static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }
}
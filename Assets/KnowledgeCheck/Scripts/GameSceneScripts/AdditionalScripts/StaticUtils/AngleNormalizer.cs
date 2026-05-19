using UnityEngine;

public static class AngleNormalizer
{
    public static void GetNormalizedOffset(ref Vector3 offset)
    {
        NormalizeAngle(ref offset.x);
        NormalizeAngle(ref offset.y);
        NormalizeAngle(ref offset.z);
    }

    public static void NormalizeAngle(ref float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
    }
}
public static class RotationUtils
{
    // 359f - так как значение float, то значения 360 градусов может и не достигнуть. Для поворачиваемой панели будет стоять ограничение в 360 градусов.
    public const float MAX_ROTATION = 360f;
    public const float MIN_ROTATION = 0f;
    public const float START_ROTATION_VALUE = 90f;
}
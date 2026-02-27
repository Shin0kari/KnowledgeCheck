using UnityEngine;

public static class TimerScript
{
    public static void SetNewTimer(ref float timer) => timer = Time.fixedDeltaTime;
    public static void UpdateTimer(ref float timer) => timer += Time.fixedDeltaTime;
}
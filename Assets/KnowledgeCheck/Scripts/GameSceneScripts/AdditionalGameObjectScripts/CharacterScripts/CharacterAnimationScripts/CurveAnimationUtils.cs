using System;
using UnityEngine;

public class CurveAnimationUtils : MonoBehaviour
{
    [SerializeField] private AnimationCurve _inAnimationCurve;
    [SerializeField] private AnimationCurve _outAnimationCurve;
    [SerializeField] private AnimationCurve _inOutAnimationCurve;

    private float _inAccCurveAverage = 0f;
    private float _outAccCurveAverage = 0f;
    private float _inOutAccCurveAverage = 0f;

    private void Awake()
    {
        ValidateAccuracyCurveAverage(_inAnimationCurve, ref _inAccCurveAverage);
        ValidateAccuracyCurveAverage(_outAnimationCurve, ref _outAccCurveAverage);
        ValidateAccuracyCurveAverage(_inOutAnimationCurve, ref _inOutAccCurveAverage);
    }

    private void ValidateAccuracyCurveAverage(AnimationCurve animationCurve, ref float accCurveAverage)
    {
        float sum = 0;
        int samples = 50;
        for (int i = 0; i < samples; i++)
        {
            sum += animationCurve.Evaluate((float)i / samples);
        }
        accCurveAverage = sum / samples;
    }

    public AnimationCurve GetAnimationCurve(CurveType curveType)
    {
        return curveType switch
        {
            CurveType.In => _inAnimationCurve,
            CurveType.InvertedIn => _outAnimationCurve,
            CurveType.Out => _outAnimationCurve,
            CurveType.InvertedOut => _inAnimationCurve,
            CurveType.InOut => _inOutAnimationCurve,
            CurveType.InvertedInOut => _inOutAnimationCurve,
            _ => null,
        };
    }

    public float GetAccuracyCurveAverage(CurveType curveType)
    {
        return curveType switch
        {
            CurveType.In => _inAccCurveAverage,
            CurveType.InvertedIn => _outAccCurveAverage,
            CurveType.Out => _outAccCurveAverage,
            CurveType.InvertedOut => _inAccCurveAverage,
            CurveType.InOut => _inOutAccCurveAverage,
            CurveType.InvertedInOut => _inOutAccCurveAverage,
            _ => 0f,
        };
    }

    public float GetAnimationCurveAbsValue(CurveType curveType, float absStartValue, ChangeValueType changeValueType, float timeEnd, float currentTime)
    {
        float newCurveValue;
        float curveValue;
        CheckCurrentTimeAndTimeEnd(ref currentTime, ref timeEnd);

        AnimationCurve animationCurve = GetAnimationCurve(curveType);
        if (curveType == CurveType.InvertedIn || curveType == CurveType.InvertedInOut || curveType == CurveType.InvertedOut)
            curveValue = 1 - animationCurve.Evaluate(currentTime / timeEnd);
        else
            curveValue = animationCurve.Evaluate(currentTime / timeEnd);

        if (changeValueType == ChangeValueType.Increase)
            newCurveValue = absStartValue + curveValue * (1 - absStartValue);
        else
            newCurveValue = absStartValue * curveValue;
        return Mathf.Clamp(newCurveValue, 0f, 1f);
    }

    public float GetAnimationCurveValue(CurveType curveType, float startValue, StraightDir dir, ChangeValueType changeValueType, float timeEnd, float currentTime)
    {
        float newCurveValue;
        float curveValue;

        CheckCurrentTimeAndTimeEnd(ref currentTime, ref timeEnd);

        float dirMatch = ((startValue == 0 && dir == StraightDir.forward) || (Mathf.Sign(startValue) == (int)dir)) ? 1 : -1;
        float remainder = Mathf.Sign(startValue) * dirMatch * (1f - dirMatch * Mathf.Abs(startValue));

        AnimationCurve animationCurve = GetAnimationCurve(curveType);
        if (curveType == CurveType.InvertedIn || curveType == CurveType.InvertedInOut || curveType == CurveType.InvertedOut)
            curveValue = 1 - animationCurve.Evaluate(currentTime / timeEnd);
        else
            curveValue = animationCurve.Evaluate(currentTime / timeEnd);

        if (changeValueType == ChangeValueType.Increase)
            newCurveValue = startValue + remainder * curveValue;
        else
            newCurveValue = Mathf.Abs(startValue) * curveValue;

        return Mathf.Clamp(newCurveValue, -1f, 1f);
    }

    public float GetAnimationCurveValue(CurveType curveType, float startValue, StrafeDir dir, ChangeValueType changeValueType, float timeEnd, float currentTime)
    {
        float newCurveValue;
        float curveValue;

        CheckCurrentTimeAndTimeEnd(ref currentTime, ref timeEnd);

        float dirMatch = ((startValue == 0 && dir == StrafeDir.right) || (Mathf.Sign(startValue) == (int)dir)) ? 1 : -1;
        float remainder = Mathf.Sign(startValue) * dirMatch * (1f - dirMatch * Mathf.Abs(startValue));

        AnimationCurve animationCurve = GetAnimationCurve(curveType);
        if (curveType == CurveType.InvertedIn || curveType == CurveType.InvertedInOut || curveType == CurveType.InvertedOut)
            curveValue = 1 - animationCurve.Evaluate(currentTime / timeEnd);
        else
            curveValue = animationCurve.Evaluate(currentTime / timeEnd);

        if (changeValueType == ChangeValueType.Increase)
            newCurveValue = startValue + remainder * curveValue;
        else
            newCurveValue = Mathf.Abs(startValue) * curveValue;

        return Mathf.Clamp(newCurveValue, -1f, 1f);
    }

    public float GetAnimationCurveValue(CurveType curveType, float currentTime, float timeEnd)
    {
        CheckCurrentTimeAndTimeEnd(ref currentTime, ref timeEnd);

        AnimationCurve animationCurve = GetAnimationCurve(curveType);
        return animationCurve.Evaluate(currentTime / timeEnd);
    }

    private static void CheckCurrentTimeAndTimeEnd(ref float currentTime, ref float timeEnd)
    {
        if (timeEnd == 0f)
        {
            Debug.LogError($"[ERROR]: in method GetAnimationCurveValue value timeEnd = {timeEnd}");
            currentTime = 1f; timeEnd = 1f;
        }
        if (currentTime > timeEnd)
            currentTime = timeEnd;
    }
}

public enum ChangeValueType
{
    Increase,
    Decrease
}
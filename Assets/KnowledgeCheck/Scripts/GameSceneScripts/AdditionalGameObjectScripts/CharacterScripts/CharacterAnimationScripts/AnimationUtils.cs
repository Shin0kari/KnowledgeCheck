using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AnimationUtils : IDisposable
{
    private float _animationDampTime = 0f;

    private Dictionary<int, int> _animationParameters = new();

    [Inject]
    private void Construct()
    {
        foreach (AnimParameter parameter in Enum.GetValues(typeof(AnimParameter)))
        {
            _animationParameters.Add((int)parameter, Animator.StringToHash(parameter.ToString()));
        }
    }

    public void Dispose()
    {
        _animationParameters.Clear();
    }

    public void SetAnimFloatValue(Animator animator, in AnimParameter parameter, in float value)
    {
        if (!_animationParameters.TryGetValue((int)parameter, out int hashParameter))
            return;

        switch (parameter)
        {
            case AnimParameter.MoveType:
                animator.SetFloat(hashParameter, value, _animationDampTime, Time.fixedDeltaTime);
                break;
            case AnimParameter.StraightMove:
                animator.SetFloat(hashParameter, value, _animationDampTime, Time.fixedDeltaTime);
                break;
            case AnimParameter.Strafe:
                animator.SetFloat(hashParameter, value, _animationDampTime, Time.fixedDeltaTime);
                break;
            case AnimParameter.Rotation:
                animator.SetFloat(hashParameter, value, _animationDampTime, Time.fixedDeltaTime);
                break;
            default:
                return;
        }
    }

    public void SetAnimTrigger(Animator animator, in AnimParameter parameter)
    {
        if (!_animationParameters.TryGetValue((int)parameter, out int hashParameter))
            return;

        animator.SetTrigger(hashParameter);
    }
}

public enum AnimParameter
{
    MoveType,
    MoveSpeed,
    StraightMove,
    Strafe,
    Rotation,
    Death,
    Drown,
    FinalDrown,
    Spawn,
    Impact,
    Hit,
    HitNoMove,
    Fall,
    Land,
    Idle
}
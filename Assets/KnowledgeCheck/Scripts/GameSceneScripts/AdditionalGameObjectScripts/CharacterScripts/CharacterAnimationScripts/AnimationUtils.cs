using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class AnimationUtils
{
    private static float _animationDampTime = 0f;
    public static void SetAnimFloatValue(Animator animator, AnimParameter parameter, float value)
    {
        switch (parameter)
        {
            case AnimParameter.MoveType:
                animator.SetFloat(parameter.ToString(), value, _animationDampTime, Time.fixedDeltaTime);
                break;
            case AnimParameter.StraightMove:
                animator.SetFloat(parameter.ToString(), value, _animationDampTime, Time.fixedDeltaTime);
                break;
            case AnimParameter.Strafe:
                animator.SetFloat(parameter.ToString(), value, _animationDampTime, Time.fixedDeltaTime);
                break;
            case AnimParameter.Rotation:
                // Debug.Log($"Set parameter {parameter}: {value}");
                animator.SetFloat(parameter.ToString(), value, _animationDampTime, Time.fixedDeltaTime);
                break;
            default:
                return;
        }
    }

    public static void SetAnimTrigger(Animator animator, AnimParameter parameters)
    {
        animator.SetTrigger(parameters.ToString());
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
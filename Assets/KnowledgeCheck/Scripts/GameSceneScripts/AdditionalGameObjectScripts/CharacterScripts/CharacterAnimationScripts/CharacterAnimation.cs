using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Animator))]
public class CharacterAnimation : MonoBehaviour
{
    private const float MAX_LAYER_WEIGHT = 1f;

    [SerializeField] private Animator _animator;

    private AnimationUtils _animationUtils;
    private int _attackAnimatorLayerIndex;

    [Inject]
    private void Construct(AnimationUtils animationUtils)
    {
        _animationUtils = animationUtils;

    }

    private void Start()
    {
        _attackAnimatorLayerIndex = _animator.GetLayerIndex("Attack");
    }

    public void SetMoveAnimValue(in Vector2 newMovementValues)
    {
        _animationUtils.SetAnimFloatValue(_animator, AnimParameter.StraightMove, newMovementValues.y);
        _animationUtils.SetAnimFloatValue(_animator, AnimParameter.Strafe, newMovementValues.x);
    }

    public void SetRotationAnimValue(in float rotationSpeed)
    {
        _animationUtils.SetAnimFloatValue(_animator, AnimParameter.Rotation, rotationSpeed);
    }

    public void SetCharacterHitAnim()
    {
        _animator.SetLayerWeight(_attackAnimatorLayerIndex, MAX_LAYER_WEIGHT);
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Hit);
    }
    public void SetCharacterHitNoMoveAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.HitNoMove);
    }
    public void SetCharacterImpactAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Impact);
    }
    public void SetCharacterIdleAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Idle);
    }
    public void SetCharacterFallAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Fall);
    }
    public void SetCharacterLandAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Land);
    }
    public void SetCharacterDeathAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Death);
    }
    public void SetCharacterDrownAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Drown);
    }
    public void SetCharacterFinalDrownAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.FinalDrown);
    }
    public void SetCharacterSpawnAnim()
    {
        _animationUtils.SetAnimTrigger(_animator, AnimParameter.Spawn);
    }

    public void ChangeAnimatorAttackLayerWeightValue(in float value)
    {
        _animator.SetLayerWeight(_attackAnimatorLayerIndex, value);
    }
}

public enum CurveType
{
    In,
    InvertedIn,
    Out,
    InvertedOut,
    InOut,
    InvertedInOut,
    EaseOutElastic,
    InvertedEaseOutBounce,
    None
}

public enum CharacterState
{
    Idle,
    Move,
    Hit,
    Impact,
    Death
}
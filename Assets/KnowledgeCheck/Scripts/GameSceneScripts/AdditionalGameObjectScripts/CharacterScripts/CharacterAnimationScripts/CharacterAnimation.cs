using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Animator))]
public class CharacterAnimation : MonoBehaviour
{
    private const float MAX_LAYER_WEIGHT = 1f;

    [SerializeField] private Animator _animator;
    private int _attackAnimatorLayerIndex;

    private void Awake()
    {
        _attackAnimatorLayerIndex = _animator.GetLayerIndex("Attack");
    }

    public void SetMoveAnimValue(Vector2 newMovementValues)
    {
        AnimationUtils.SetAnimFloatValue(_animator, AnimParameter.StraightMove, newMovementValues.y);
        AnimationUtils.SetAnimFloatValue(_animator, AnimParameter.Strafe, newMovementValues.x);
    }

    public void SetRotationAnimValue(float rotationSpeed)
    {
        AnimationUtils.SetAnimFloatValue(_animator, AnimParameter.Rotation, rotationSpeed);
    }

    public void SetCharacterHitAnim()
    {

        _animator.SetLayerWeight(_attackAnimatorLayerIndex, MAX_LAYER_WEIGHT);
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Hit);
    }
    public void SetCharacterHitNoMoveAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.HitNoMove);
    }
    public void SetCharacterImpactAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Impact);
    }
    public void SetCharacterIdleAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Idle);
    }
    public void SetCharacterFallAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Fall);
    }
    public void SetCharacterLandAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Land);
    }
    public void SetCharacterDeathAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Death);
    }
    public void SetCharacterDrownAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Drown);
    }
    public void SetCharacterFinalDrownAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.FinalDrown);
    }
    public void SetCharacterSpawnAnim()
    {
        AnimationUtils.SetAnimTrigger(_animator, AnimParameter.Spawn);
    }

    public void ChangeAnimatorAttackLayerWeightValue(float value)
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
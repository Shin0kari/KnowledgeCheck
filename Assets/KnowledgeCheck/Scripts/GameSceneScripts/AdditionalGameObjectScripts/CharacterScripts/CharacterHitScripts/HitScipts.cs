using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public abstract class HitScipts : MonoBehaviour
{
    [SerializeField] protected CharacterAnimation _characterAnimation;
    protected bool _isAttackEnded = true;

    protected virtual void Attack()
    {
        if (_isAttackEnded)
        {
            _characterAnimation.SetCharacterHitAnim();
            _isAttackEnded = false;
        }
    }

    public virtual void EndAttack()
    {
        _isAttackEnded = true;
        _characterAnimation.ChangeAnimatorAttackLayerWeightValue(0f);
    }
}
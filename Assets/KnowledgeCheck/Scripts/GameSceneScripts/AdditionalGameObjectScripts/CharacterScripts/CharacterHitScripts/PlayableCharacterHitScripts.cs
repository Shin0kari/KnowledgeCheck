using System;
using UnityEngine.InputSystem;
using Zenject;

public class PlayableCharacterHitScripts : HitScipts, IAttackSignalSender
{
    private PlayerInputSystem _playerInput;

    public event Action OnStartAttack;
    public event Action OnEndAttack;

    [Inject]
    private void Construct()
    {
        _playerInput = new();

        _playerInput.Player.Attack.performed += OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Attack();
    }

    protected override void Attack()
    {
        if (_isAttackEnded)
        {
            OnStartAttack?.Invoke();
            _characterAnimation.SetCharacterHitAnim();
            _isAttackEnded = false;
        }
    }

    public override void EndAttack()
    {
        OnEndAttack?.Invoke();
        _isAttackEnded = true;
        _characterAnimation.ChangeAnimatorAttackLayerWeightValue(0f);
    }

    private void OnEnable()
    {
        _playerInput?.Enable();
    }

    private void OnDisable()
    {
        _playerInput?.Disable();
    }
}
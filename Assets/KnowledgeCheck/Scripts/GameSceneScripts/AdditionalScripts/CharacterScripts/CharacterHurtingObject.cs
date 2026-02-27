using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CharacterHurtingObject : HurtingObject, IDisposable
{
    [SerializeField] private CharacterEventObserver _characterEventObserver;
    private IAttackSignalSender _attackSignalSender;
    private Collider _hurtingCollider;

    private void Awake()
    {
        _attackSignalSender = GetComponentInParent<IAttackSignalSender>();
        _hurtingCollider = GetComponent<Collider>();

        if (_attackSignalSender != null)
        {
            _attackSignalSender.OnStartAttack += EnableCollider;
            _attackSignalSender.OnEndAttack += DisableCollider;
        }

        if (_characterEventObserver != null)
        {
            _characterEventObserver.OnDeath += DisableCollider;
            _characterEventObserver.OnSpawn += EnableCollider;
        }
    }

    public void Dispose()
    {
        if (_attackSignalSender != null)
        {
            _attackSignalSender.OnStartAttack -= EnableCollider;
            _attackSignalSender.OnEndAttack -= DisableCollider;
        }
        if (_characterEventObserver != null)
        {
            _characterEventObserver.OnDeath += DisableCollider;
            _characterEventObserver.OnSpawn += EnableCollider;
        }
    }

    private void DisableCollider()
    {
        _hurtingCollider.enabled = false;
    }

    private void EnableCollider()
    {
        _hurtingCollider.enabled = true;
    }
}
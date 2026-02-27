using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CharacterHeartBox : HeartBox
{
    [SerializeField] private CharacterEventObserver _characterEventObserver;
    private Collider _hurtingCollider;

    protected override void Awake()
    {
        base.Awake();

        _hurtingCollider = GetComponent<Collider>();

        _characterEventObserver.OnDeath += DisableCollider;
        _characterEventObserver.OnSpawn += EnableCollider;
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
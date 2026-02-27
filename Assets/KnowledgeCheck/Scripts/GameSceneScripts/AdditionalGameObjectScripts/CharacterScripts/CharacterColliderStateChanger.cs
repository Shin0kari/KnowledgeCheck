using System;
using UnityEngine;

[RequireComponent(typeof(CharacterEventObserver))]
public class CharacterColliderStateChanger : MonoBehaviour, IDisposable
{
    [SerializeField] private Collider _characterCollider;
    private CharacterEventObserver _characterEventObserver;
    [SerializeField] private LayerMask _charactersLayerMask;
    private LayerMask _nothingLayerMask;

    private void Awake()
    {
        _nothingLayerMask = LayerMask.GetMask("Nothing");

        _characterEventObserver = GetComponent<CharacterEventObserver>();
        _characterEventObserver.OnDeath += OffCollider;
        _characterEventObserver.OnSpawn += OnCollider;
    }

    public void Dispose()
    {
        if (_characterEventObserver != null)
        {
            _characterEventObserver.OnDeath -= OffCollider;
            _characterEventObserver.OnSpawn -= OnCollider;
        }
    }

    private void OnCollider()
    {
        // _characterCollider.enabled = true;
        _characterCollider.excludeLayers = _nothingLayerMask;
    }

    private void OffCollider()
    {
        // _characterCollider.enabled = false;
        _characterCollider.excludeLayers = _charactersLayerMask;
    }
}
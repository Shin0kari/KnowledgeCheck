using System;
using UnityEngine;

public class HeartBox : MonoBehaviour
{
    protected IDamagable _damagableObject;
    public event Action OnGetDamage;

    protected virtual void Awake()
    {
        _damagableObject = GetComponentInParent<IDamagable>();
    }

    private void OnDestroy()
    {
        OnGetDamage = null;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (_damagableObject != null && other.TryGetComponent<HurtingObject>(out var hurtingObject))
        {
            _damagableObject.ChangeHealth(hurtingObject.Damage);
            OnGetDamage?.Invoke();
        }
    }
}

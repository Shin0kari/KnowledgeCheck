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

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            _damagableObject.ChangeHealth(other.GetComponent<HurtingObject>().Damage);
            OnGetDamage?.Invoke();
        }
    }
}

using System;

public interface IDamagable
{
    public event Action<float> HealthChanged;

    public float GetHealth();
    public void ChangeHealth(in float value);

    public void ClearActions();
}
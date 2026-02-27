using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(HealthBarLifeTimeController))]
public class HealthBar : MonoBehaviour, IDisposable
{
    private const float LOWER_HEALTH_VALUE_RANGE = 0f;
    private const float UPPER_HEALTH_VALUE_RANGE = 1f;
    // не const, чтобы можно было бы иметь разные по размеру HPBar
    // private float _upperHealthValueRange;

    [SerializeField] private float _maxHealthValue = 100f; // К примеру 10тыс здоровья на граничные значения от 0 до 78
    [SerializeField] private float _canvasRotationSpeed = 10f;
    private float _healthValue = 100f; // текущее значение здоровья
    private Image _healthBarImage;
    private HealthBarLifeTimeController _lifeTimeController;

    private CameraUtils _cameraUtils;

    private IDamagable _damagableObject;

    [Inject]
    private void Construct(CameraUtils cameraUtils)
    {
        _lifeTimeController = GetComponent<HealthBarLifeTimeController>();
        _cameraUtils = cameraUtils;
        _healthBarImage = gameObject.GetComponentInChildren<HealthBarUtility>().GetHealthFilled();
    }

    public void Dispose()
    {
        _damagableObject.HealthChanged -= UpdateHealthBar;
    }

    public void SetDamagableObject(IDamagable damagableObject)
    {
        _damagableObject = damagableObject;

        _damagableObject.HealthChanged += UpdateHealthBar;

        if (_damagableObject is Enemy)
        {
            var addedComponent = gameObject.AddComponent<WorldHealthRotation>();
            addedComponent.Construct(_canvasRotationSpeed, _cameraUtils);
        }
    }

    public float GetHealthValue()
    {
        return _healthValue;
    }

    public float GetMaxHealthValue()
    {
        return _maxHealthValue;
    }

    public void UpdateHealthBar(float currentHealt)
    {
        if (currentHealt <= 0)
            _lifeTimeController.OffHealthBar();
        else
        {
            if (!_lifeTimeController.CheckHealthBarState())
                _lifeTimeController.OnHealthBar();
        }

        UpdateHealthBarValue(currentHealt);
    }

    private void UpdateHealthBarValue(float currentHealt)
    {
        _healthBarImage.fillAmount = currentHealt / _maxHealthValue * UPPER_HEALTH_VALUE_RANGE;
    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, HealthBar> { }
}
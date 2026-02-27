using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CharacterTimer : MonoBehaviour, IDisposable
{
    private const float MIN_TIME = 0f;
    private float _underWaterTimer = MIN_TIME;

    private bool _isStartUnderWaterTimer;
    private bool _isDrowned;

    public event Action CharacterDrowning;
    public event Action CharacterDrowned;

    [SerializeField] private CharacterPhysicsUpdater _characterPhysics;
    [SerializeField] private float _duringDrowningTime;
    [SerializeField] private bool _canDrown;

    private void Awake()
    {
        _characterPhysics.OnUnderWaterStateChange += ChangeUnderWaterState;
    }

    public void Dispose()
    {
        _characterPhysics.OnUnderWaterStateChange -= ChangeUnderWaterState;
    }

    private void Update()
    {
        UpdateUnderWaterTimer();
    }

    private void ChangeUnderWaterState(bool currentState)
    {
        if (currentState != _isStartUnderWaterTimer)
        {
            _underWaterTimer = MIN_TIME;
            _isStartUnderWaterTimer = currentState;
            if (_isStartUnderWaterTimer && _canDrown)
                SetDrowningSignal();
        }
    }

    private void UpdateUnderWaterTimer()
    {
        if (!_isStartUnderWaterTimer || !_canDrown)
            return;

        if (_underWaterTimer >= _duringDrowningTime && !_isDrowned)
            SetFinalDrowningSignal();

        _underWaterTimer += Time.deltaTime;
    }

    private void SetFinalDrowningSignal()
    {
        _isDrowned = true;

        _isStartUnderWaterTimer = false;
        _underWaterTimer = MIN_TIME;

        CharacterDrowned?.Invoke();
    }

    private void SetDrowningSignal()
    {
        if (!_isDrowned)
            CharacterDrowning?.Invoke();
    }
}
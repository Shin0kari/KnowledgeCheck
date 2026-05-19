using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class ArenaTimer : ITickable, IDisposable
{
    private const float MIN_TIME = 0f;

    private IAssetProviderGetter _assetProvider;

    private float _timer = MIN_TIME;
    private bool _isBattleStarted = false;
    private bool _hasStartSpawnSignalSent = false;
    private bool _hasStopSpawnSignalSent = false;
    private bool _hasOnWaterValveSignalSent = false;

    private ArenaTimeStruct _battleTime;

    public event Action EndTimer;
    public event Action StartSpawnSignal;
    public event Action StopSpawnSignal;
    public event Action SetOnWaterPipeSignal;

    private DisposableBag _dB;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        _dB.Dispose();

        EndTimer = null;
        StartSpawnSignal = null;
        StopSpawnSignal = null;
        SetOnWaterPipeSignal = null;
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ArenaUtils>()
            .OfType<IBindingSingletonComponent, ArenaUtils>()
            .Subscribe(arenaUtils =>
            {
                if (arenaUtils == null)
                    return;
                _battleTime = arenaUtils.BattleTime;
            })
            .AddTo(ref _dB);
    }

    public void Tick()
    {
        if (!_isBattleStarted)
            return;

        if (_timer < _battleTime.durationSecBattleTime)
        {
            _timer += Time.deltaTime;

            if (!_hasStartSpawnSignalSent &&
                _timer >= _battleTime.spawnStartOffsetSecTime)
            {
                SendStartSpawnSignal();
            }
            else if (!_hasStopSpawnSignalSent &&
                _timer >= _battleTime.durationSecBattleTime - _battleTime.spawnStopOffsetSecTime)
            {
                SendStopSpawnSignal();
            }

            if (!_hasOnWaterValveSignalSent &&
                _timer >= _battleTime.onWaterValveSecTime)
            {
                SendOnWaterValveSignal();
            }

        }
        else
            SendEndTimerSignal();
    }

    public void StartTimer()
    {
        _timer = MIN_TIME;
        _isBattleStarted = true;
        _hasStartSpawnSignalSent = false;
        _hasStopSpawnSignalSent = false;
        _hasOnWaterValveSignalSent = false;
    }

    private void SendEndTimerSignal()
    {
        _isBattleStarted = false;
        _hasStartSpawnSignalSent = false;
        _hasStopSpawnSignalSent = false;
        EndTimer?.Invoke();
    }

    private void SendStartSpawnSignal()
    {
        _hasStartSpawnSignalSent = true;
        StartSpawnSignal?.Invoke();
    }

    private void SendStopSpawnSignal()
    {
        _hasStopSpawnSignalSent = true;
        StopSpawnSignal?.Invoke();
    }

    private void SendOnWaterValveSignal()
    {
        _hasOnWaterValveSignalSent = true;
        SetOnWaterPipeSignal?.Invoke();
    }
}
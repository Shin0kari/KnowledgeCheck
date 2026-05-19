using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class ArenaController : IInitializable, IDisposable
{
    private IAssetProviderGetter _assetProvider;

    private ArenaGateController _gateController;
    private ArenaTimer _arenaTimer;
    private SignalBus _signalBus;

    private ButtonArenaStateToggle _arenaStateToggle;
    private bool _isSubscibed = false;
    private bool _endArenaAvailable = true;

    public event Action StartArenaBattle;
    public event Action StopArenaBattle;
    public event Action StartSpawnEnemy;
    public event Action StopSpawnEnemy;
    public event Action SetOnWaterPipe;

    private DisposableBag _dB;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        ArenaTimer arenaTimer,
        SignalBus signalBus)
    {
        _assetProvider = assetProvider;
        _arenaTimer = arenaTimer;
        _signalBus = signalBus;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetSubscribesOnPlayer);
        if (_isSubscibed && _arenaStateToggle != null)
        {
            _arenaStateToggle.BattleStarted -= StartBattle;
            _arenaStateToggle.BattleStoped -= StopBattle;
        }

        if (_arenaTimer != null)
        {
            _arenaTimer.StartSpawnSignal -= SendStartSpawnSignal;
            _arenaTimer.StopSpawnSignal -= SendStopSpawnSignal;
            _arenaTimer.SetOnWaterPipeSignal -= SendSetOnWaterPipeSignal;
            _arenaTimer.EndTimer -= SendEndTimerSignal;
        }

        _dB.Dispose();

        StartArenaBattle = null;
        StopArenaBattle = null;
        StartSpawnEnemy = null;
        StopSpawnEnemy = null;
        SetOnWaterPipe = null;
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ArenaGateController>()
            .OfType<IBindingSingletonComponent, ArenaGateController>()
            .Subscribe(gateController =>
            {
                if (gateController == null)
                    return;
                _gateController = gateController;
            })
            .AddTo(ref _dB);
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayerSpawnedSignal>(SetSubscribesOnPlayer);
        _arenaTimer.StartSpawnSignal += SendStartSpawnSignal;
        _arenaTimer.StopSpawnSignal += SendStopSpawnSignal;
        _arenaTimer.SetOnWaterPipeSignal += SendSetOnWaterPipeSignal;
        _arenaTimer.EndTimer += SendEndTimerSignal;
    }

    private void SendStartSpawnSignal()
    {
        StartSpawnEnemy?.Invoke();
    }
    private void SendStopSpawnSignal()
    {
        StopSpawnEnemy?.Invoke();
    }

    private void SendSetOnWaterPipeSignal()
    {
        SetOnWaterPipe?.Invoke();
    }

    private void SendEndTimerSignal()
    {
        if (_endArenaAvailable)
            StopArenaBattle?.Invoke();
    }

    public void OnPlayerDeath()
    {
        _endArenaAvailable = false;
    }
    public void OnPlayerWin()
    {
        _endArenaAvailable = false;
    }

    private void SetSubscribesOnPlayer(PlayerSpawnedSignal args)
    {

        _arenaStateToggle = args.Player.GetComponent<ButtonArenaStateToggle>();
        _arenaStateToggle.BattleStarted += StartBattle;
        _arenaStateToggle.BattleStoped += StopBattle;

        _isSubscibed = true;
    }

    public void StartBattle()
    {
        _arenaTimer.StartTimer();
        _gateController?.CloseGate();
        StartArenaBattle?.Invoke();
    }

    public void StopBattle()
    {
        _gateController?.OpenGate();
        StopArenaBattle?.Invoke();
    }

}
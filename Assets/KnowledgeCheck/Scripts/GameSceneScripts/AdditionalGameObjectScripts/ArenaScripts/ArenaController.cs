using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ArenaController : IInitializable, IDisposable
{
    private ArenaGateController _arenaController;
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

    [Inject]
    private void Construct(
        ArenaGateController arenaController,
        ArenaTimer arenaTimer,
        SignalBus signalBus)
    {
        _arenaController = arenaController;
        _arenaTimer = arenaTimer;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayerSpawnedSignal>(SetSubscribesOnPlayer);
        _arenaTimer.StartSpawnSignal += SendStartSpawnSignal;
        _arenaTimer.StopSpawnSignal += SendStopSpawnSignal;
        _arenaTimer.SetOnWaterPipeSignal += SendSetOnWaterPipeSignal;
        _arenaTimer.EndTimer += SendEndTimerSignal;
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetSubscribesOnPlayer);
        if (_isSubscibed)
        {
            _arenaStateToggle.BattleStarted -= StartBattle;
            _arenaStateToggle.BattleStoped -= StopBattle;
        }

        _arenaTimer.StartSpawnSignal -= SendStartSpawnSignal;
        _arenaTimer.StopSpawnSignal -= SendStopSpawnSignal;
        _arenaTimer.SetOnWaterPipeSignal -= SendSetOnWaterPipeSignal;
        _arenaTimer.EndTimer -= SendEndTimerSignal;
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
        _arenaController.CloseGate();
        StartArenaBattle?.Invoke();
    }

    public void StopBattle()
    {
        _arenaController.OpenGate();
        StopArenaBattle?.Invoke();
    }

}
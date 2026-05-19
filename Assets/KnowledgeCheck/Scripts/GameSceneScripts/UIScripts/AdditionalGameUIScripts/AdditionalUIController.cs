using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AdditionalUIController : IDisposable
{
    private SignalBus _signalBus;
    private Player _player;

    private ArenaController _arenaController;
    private PlayerEventObserver _characterEventObserver;
    private WinUI _winUI;
    private LoseUI _loseUI;

    private bool _playerDeathAvailable = true;
    private bool _isPlayerDeath = false;

    [Inject]
    private void Construct(
        SignalBus signalBus,
        ArenaController arenaController,
        WinUI winUI,
        LoseUI loseUI
    )
    {
        _signalBus = signalBus;
        _arenaController = arenaController;
        _winUI = winUI;
        _loseUI = loseUI;

        _signalBus.Subscribe<PlayerSpawnedSignal>(SetPlayerEventObserver);

        _arenaController.StopSpawnEnemy += OnEndSpawn;
        _arenaController.StopArenaBattle += OnEndBattle;
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<PlayerSpawnedSignal>(SetPlayerEventObserver);
        if (_arenaController != null)
        {
            _arenaController.StopSpawnEnemy -= OnEndSpawn;
            _arenaController.StopArenaBattle -= OnEndBattle;
        }
        if (_characterEventObserver != null)
        {
            _characterEventObserver.OnDeathState -= OnDeath;
            _characterEventObserver.OnDrownState -= OnDrown;
        }
    }

    private void SetPlayerEventObserver(PlayerSpawnedSignal args)
    {
        _player = args.Player;

        _characterEventObserver = _player.GetComponent<PlayerEventObserver>();

        _characterEventObserver.OnDeathState += OnDeath;
        _characterEventObserver.OnDrownState += OnDrown;
    }

    private void OnEndSpawn()
    {
        _winUI.ChangeFadeWhiteWindow().Forget();
    }

    private void OnEndBattle()
    {
        if (_isPlayerDeath)
            return;

        _playerDeathAvailable = false;
        CursorVisibility.OnAlwaysCursorVisibility();
        _winUI.Win().Forget();
    }

    private void OnDeath()
    {
        if (!_playerDeathAvailable)
            return;

        _isPlayerDeath = true;
        CursorVisibility.OnAlwaysCursorVisibility();
        _loseUI.OnDeath().Forget();
    }

    private void OnDrown()
    {
        if (!_playerDeathAvailable)
            return;

        _isPlayerDeath = true;
        CursorVisibility.OnAlwaysCursorVisibility();
        _loseUI.OnDrown().Forget();
    }
}
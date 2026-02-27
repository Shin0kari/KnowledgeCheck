using System;
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
        _signalBus.Unsubscribe<PlayerSpawnedSignal>(SetPlayerEventObserver);
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
        _winUI.ChangeFadeWhiteWindow();
    }

    private void OnEndBattle()
    {
        _playerDeathAvailable = false;

        CursorVisibility.OnAlwaysCursorVisibility();
        _winUI.Win();
    }

    private void OnDeath()
    {
        if (_playerDeathAvailable)
        {
            CursorVisibility.OnAlwaysCursorVisibility();
            _loseUI.OnDeath();
        }
    }

    private void OnDrown()
    {
        if (_playerDeathAvailable)
        {
            CursorVisibility.OnAlwaysCursorVisibility();
            _loseUI.OnDrown();
        }
    }
}
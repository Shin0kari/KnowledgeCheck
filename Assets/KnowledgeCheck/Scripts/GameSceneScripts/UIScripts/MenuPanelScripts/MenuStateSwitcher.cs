using System;
using UnityEngine;
using Zenject;

public class MenuStateSwitcher : IDisposable
{
    private SignalBus _signalBus;
    private ArenaController _arenaController;
    private MainMenu _mainMenu;

    private Player _player;
    private PlayerEventObserver _characterEventObserver;

    [Inject]
    private void Construct(
        SignalBus signalBus,
        ArenaController arenaController,
        MainMenu mainMenu
        )
    {
        _signalBus = signalBus;
        _arenaController = arenaController;
        _mainMenu = mainMenu;

        _signalBus.Subscribe<PlayerSpawnedSignal>(SetPlayerEventObserver);

        _arenaController.StopArenaBattle += OffMenuAvailable;
    }

    private void SetPlayerEventObserver(PlayerSpawnedSignal args)
    {
        _player = args.Player;

        _characterEventObserver = _player.GetComponent<PlayerEventObserver>();

        _characterEventObserver.OnDeath += OffMenuAvailable;
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayerSpawnedSignal>(SetPlayerEventObserver);
        if (_arenaController != null)
        {
            _arenaController.StopArenaBattle -= OffMenuAvailable;
        }
        if (_characterEventObserver != null)
        {
            _characterEventObserver.OnDeath -= OffMenuAvailable;
        }
    }

    private void OffMenuAvailable()
    {
        _mainMenu.ChangeMenuAvailableState(false);
    }
}
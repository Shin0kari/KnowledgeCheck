using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class MenuStateSwitcher : IDisposable
{
    private SignalBus _signalBus;
    private IAssetProviderGetter _assetProvider;
    private ArenaController _arenaController;
    private MenuController _menu;

    private Player _player;
    private PlayerEventObserver _characterEventObserver;

    private DisposableBag _dB;

    [Inject]
    private void Construct(
        SignalBus signalBus,
        IAssetProviderGetter assetProvider,
        ArenaController arenaController
        )
    {
        _signalBus = signalBus;
        _assetProvider = assetProvider;
        _arenaController = arenaController;

        SubscribeOnUpdateObjects();

        _signalBus.Subscribe<PlayerSpawnedSignal>(SetPlayerEventObserver);

        _arenaController.StopArenaBattle += OffMenuAvailable;
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<PlayerSpawnedSignal>(SetPlayerEventObserver);
        if (_arenaController != null)
        {
            _arenaController.StopArenaBattle -= OffMenuAvailable;
        }
        if (_characterEventObserver != null)
        {
            _characterEventObserver.OnDeath -= OffMenuAvailable;
        }

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<MenuController>()
            .OfType<IBindingSingletonComponent, MenuController>()
            .Subscribe(menu =>
            {
                if (menu == null)
                    return;
                _menu = menu;
            })
            .AddTo(ref _dB);
    }

    private void SetPlayerEventObserver(PlayerSpawnedSignal args)
    {
        _player = args.Player;

        _characterEventObserver = _player.GetCharacterEventObserver();

        _characterEventObserver.OnDeath += OffMenuAvailable;
    }

    private void OffMenuAvailable()
    {
        if (_menu != null)
            _menu.ChangeMenuAvailableState(false);
    }
}
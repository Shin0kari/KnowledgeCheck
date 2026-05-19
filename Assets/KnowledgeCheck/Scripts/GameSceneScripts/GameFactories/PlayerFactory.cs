using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class PlayerFactory : AbstractFactoryStarter, IInitializable, IDisposable
{
    private IAddressablesProvider _aP; // addressablesProvider
    private SceneCharactersSettingsRepository _sceneCharactersSettingsRepository;
    private SceneCharactersSettingsSO _sceneCharactersSettingsSO;

    private ReactiveProperty<BaseCharacterSettingsSO> _reactivePlayerCharacterSettingsSO;
    private BaseCharacterSettingsSO _playerSettingsSO;
    private GameObject _playerPrefab;

    private Player _player;
    private readonly Player.Factory _playerFactory;

    private readonly SignalBus _signalBus;

    private DisposableBag _dB;

    private UniTaskCompletionSource _playerSettingsSOLoadedSource = new();
    private UniTaskCompletionSource _playerPrefabLoadedSource = new();
    private CancellationTokenSource _ct = new();

    public PlayerFactory(
        IAddressablesProvider aP,
        SceneCharactersSettingsRepository sceneCharactersSettingsRepository,
        Player.Factory playerFactory,
        SignalBus signalBus
    )
    {
        _sceneCharactersSettingsRepository = sceneCharactersSettingsRepository;
        _aP = aP;

        _playerFactory = playerFactory;
        _signalBus = signalBus;

        AsyncLoadResource().Forget();
        AsyncSetPlayerPrefab().Forget();
    }

    public void Dispose()
    {
        _playerSettingsSOLoadedSource.TrySetCanceled();
        _playerSettingsSOLoadedSource = null;

        _playerPrefabLoadedSource.TrySetCanceled();
        _playerPrefabLoadedSource = null;

        _signalBus.LateDispose();

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private async UniTask AsyncLoadResource()
    {
        _sceneCharactersSettingsSO = await _sceneCharactersSettingsRepository.AsyncGetSceneCharactersSettingsSO(_ct.Token);

        await LoadCharacterSettingsSOResources<WarriorSettingsSO>(_sceneCharactersSettingsSO);
    }

    private async UniTask LoadCharacterSettingsSOResources<T>(SceneCharactersSettingsSO sceneCharactersSettingsSO) where T : BaseCharacterSettingsSO
    {
        _reactivePlayerCharacterSettingsSO = await sceneCharactersSettingsSO.GetPlayerSettings(typeof(T), _ct.Token);

        _reactivePlayerCharacterSettingsSO?
            .OfType<BaseCharacterSettingsSO, T>()
            .Subscribe(playerSettingsSO =>
            {
                if (playerSettingsSO == null)
                    return;

                _playerSettingsSO = playerSettingsSO;
                _playerSettingsSOLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    public async UniTask AsyncSetPlayerPrefab()
    {
        await _playerSettingsSOLoadedSource.Task.AttachExternalCancellation(_ct.Token);
        _playerPrefab = await _aP.AsyncGetAddressablesDataFromReference<GameObject>(_playerSettingsSO.CharacterPrefab, _ct.Token);
        _playerPrefabLoadedSource.TrySetResult();
    }

    private void SetDefaultPlayerData(Player player, BaseCharacterSettingsSO settingsSO)
    {
        CharacterStats characterStats = settingsSO.CharacterBaseData.Stats with { };
        CharacterAffects characterAffects = settingsSO.CharacterBaseData.Affects with { };
        Inventory characterInventory = settingsSO.CharacterBaseData.Inventory with { };

        player.SetCharacterData(
            characterStats,
            characterAffects,
            characterInventory,
            settingsSO.CharacterType,
            settingsSO.CharacterName);
    }

    // Спавн игрока
    public void Initialize()
    {
        if (!_isFactoryActive)
        {
            return;
        }
        if (_player != null)
        {
            return;
        }

        AsyncSpawnPlayer().Forget();
    }

    private async UniTask AsyncSpawnPlayer()
    {
        try
        {
            await _playerPrefabLoadedSource.Task.AttachExternalCancellation(_ct.Token);

            _player = _playerFactory.Create(_playerPrefab);
            SetDefaultPlayerData(_player, _playerSettingsSO);

            _signalBus.Fire(new PlayerSpawnedSignal() { Player = _player });
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    private void DespawnPlayer(IDamagable player)
    {
        GameObject.Destroy((player as Player).gameObject);
    }
}

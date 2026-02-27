using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerFactory : AbstractFactoryStarter, IInitializable
{
    private GameObject _playerPrefab;
    private Player _player;
    private readonly Player.Factory _playerFactory;

    private readonly SignalBus _signalBus;
    // private readonly CameraUtils _cameraUtils;

    public PlayerFactory(
        GameObject playerPrefab,
        Player.Factory playerFactory,
        // CameraUtils cameraUtils
        SignalBus signalBus
    )
    {
        _playerPrefab = playerPrefab;
        _playerFactory = playerFactory;
        _signalBus = signalBus;

        // _cameraUtils = cameraUtils;
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

        _player = _playerFactory.Create(_playerPrefab);

        _signalBus.Fire(new PlayerSpawnedSignal() { Player = _player });
        // _playerProvider.Player = _player;

        // _cameraUtils.SetCinemachineCameraTarget(_player.gameObject);
    }

    private void DespawnPlayer(IDamagable player)
    {
        GameObject.Destroy((player as Player).gameObject);
    }
}

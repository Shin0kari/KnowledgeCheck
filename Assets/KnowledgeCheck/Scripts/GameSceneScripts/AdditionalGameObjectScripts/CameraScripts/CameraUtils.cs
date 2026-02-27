using System;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class CameraUtils : MonoBehaviour, IDisposable
{
    [SerializeField] private CinemachineCamera _freeLookCamera;
    [SerializeField] private CinemachineCamera _thirdPersonCamera;
    [SerializeField] private CinemachineCamera _TopDownLookCamera;

    private SignalBus _signalBus;
    private PlayerEventObserver _playerEventObserver;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;

        _signalBus.Subscribe<PlayerSpawnedSignal>(SetPlayer);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayerSpawnedSignal>(SetPlayer);

        ClearPlayerSubscribes();
    }

    private void SetPlayer(PlayerSpawnedSignal args)
    {
        _playerEventObserver = args.Player.GetComponent<PlayerEventObserver>();

        _playerEventObserver.OnSpawn += SetPlayerTarget;
    }

    private void SetPlayerTarget()
    {
        ClearPlayerSubscribes();

        SetCinemachineCameraTarget(_playerEventObserver.gameObject);
    }

    private void ClearPlayerSubscribes()
    {
        if (_playerEventObserver != null)
            _playerEventObserver.OnSpawn -= SetPlayerTarget;
    }

    public CinemachineCamera GetCinemachineCamera(CameraTypes type) => type switch
    {
        CameraTypes.FreeLookView => _freeLookCamera,
        CameraTypes.ThirdPersonView => _thirdPersonCamera,
        CameraTypes.TopDownView => _TopDownLookCamera,
        _ => _freeLookCamera,
    };

    public void SetCinemachineCameraTarget(GameObject target)
    {
        foreach (CameraTypes cameraType in Enum.GetValues(typeof(CameraTypes)))
        {
            GetCinemachineCamera(cameraType).Target.TrackingTarget = target.transform;
        }
    }
}
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class FreeLookCameraPosController : IInitializable, IFixedTickable
{
    public bool Enabled { get; set; } = false;
    private CameraUtils _cameraUtils;

    private CinemachineCamera _freeLookCamera;
    private CinemachineCamera _thirdPersonCamera;

    private CinemachineOrbitalFollow _cinemachineOrbitalFollower;

    [Inject]
    private void Construct(CameraUtils cameraUtils)
    {
        _cameraUtils = cameraUtils;
    }

    public void Initialize()
    {
        _freeLookCamera = _cameraUtils.GetCinemachineCamera(CameraTypes.FreeLookView);
        _thirdPersonCamera = _cameraUtils.GetCinemachineCamera(CameraTypes.ThirdPersonView);

        if (_freeLookCamera != null)
            _cinemachineOrbitalFollower = _freeLookCamera.GetComponent<CinemachineOrbitalFollow>();
    }

    public void FixedTick()
    {
        if (Enabled)
        {
            _cinemachineOrbitalFollower
                .HorizontalAxis
                .Value
            = _thirdPersonCamera
                .gameObject
                .transform
                .rotation
                .eulerAngles
                .y;
        }
    }
}
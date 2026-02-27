using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class FreeLookCameraPosController : IInitializable, IFixedTickable
{
    public bool Enabled { get; set; } = false;
    private CameraUtils _cameraUtils;

    private CinemachineCamera _freeLookCamera;
    private CinemachineCamera _thirdPersonCamera;

    [Inject]
    private void Construct(CameraUtils cameraUtils)
    {
        _cameraUtils = cameraUtils;
    }

    public void Initialize()
    {
        _freeLookCamera = _cameraUtils.GetCinemachineCamera(CameraTypes.FreeLookView);
        _thirdPersonCamera = _cameraUtils.GetCinemachineCamera(CameraTypes.ThirdPersonView);
    }

    public void FixedTick()
    {
        if (Enabled)
        {
            _freeLookCamera
                .GetComponent<CinemachineOrbitalFollow>()
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
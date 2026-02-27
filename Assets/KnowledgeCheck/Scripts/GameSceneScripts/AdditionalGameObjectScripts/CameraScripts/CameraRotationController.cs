using System;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CinemachineInputAxisController))]
public class CameraRotationController : MonoBehaviour, IDisposable
{
    private CinemachineInputAxisController _axisController;
    private IChangeStateMenuSender _menuStateSender;


    [Inject]
    private void Construct(IChangeStateMenuSender menuStateSender)
    {
        _menuStateSender = menuStateSender;

        _axisController = GetComponent<CinemachineInputAxisController>();

        _menuStateSender.ChangeState += ChangeCameraControlState;
    }

    private void ChangeCameraControlState(bool isMenuOn)
    {
        _axisController.enabled = !isMenuOn;
    }

    public void Dispose()
    {
        if (_menuStateSender != null)
            _menuStateSender.ChangeState -= ChangeCameraControlState;
    }
}
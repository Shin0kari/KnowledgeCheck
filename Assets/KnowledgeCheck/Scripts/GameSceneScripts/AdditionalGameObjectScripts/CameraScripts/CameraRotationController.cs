using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CinemachineInputAxisController))]
public class CameraRotationController : MonoBehaviour
{
    private IAssetProviderGetter _assetProvider;

    private CinemachineInputAxisController _axisController;
    private IChangeStateMenuSender _menuStateSender;

    private DisposableBag _dB;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        _axisController = GetComponent<CinemachineInputAxisController>();

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        if (_menuStateSender != null)
            _menuStateSender.ChangeState -= ChangeCameraControlState;

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<IChangeStateMenuSender>()
            .OfType<IBindingSingletonComponent, IChangeStateMenuSender>()
            .Subscribe(menuStateSender =>
            {
                if (menuStateSender == null)
                    return;

                if (_menuStateSender != null)
                    _menuStateSender.ChangeState -= ChangeCameraControlState;

                _menuStateSender = menuStateSender;
                _menuStateSender.ChangeState += ChangeCameraControlState;
            })
            .AddTo(ref _dB);
    }

    private void ChangeCameraControlState(bool isMenuOn)
    {
        _axisController.enabled = !isMenuOn;
    }
}
using System;
using UnityEngine;
using Zenject;

public class PlayerControl : IInitializable, IDisposable
{
    private SignalBus _signalBus;

    // Необходим, чтобы при выключении ThirdPersonView 
    // камера FreeLookView находилась на том же значении HorizontalAxis
    private FreeLookCameraPosController _freeLookCameraPosController;
    private ArenaController _arenaController;
    private IChangeStateMenuSender _menuStateSender;

    private ButtonThirdPersonView _buttonThirdPersonView;
    private FreeLookViewScript _freeLookView;
    private ThirdPersonViewScript _thirdPersonView;
    private TopDownViewScript _topDownView;
    private PlayableCharacterHitScripts _characterHit;
    private ButtonArenaStateToggle _arenaStateToggle;
    private PlayerEventObserver _characterEventObserver;

    private AbstractViewScript _currentViewControl;
    private CameraTypes _currentViewMode;

    private bool _isControlOn;
    private bool _isActionsSet = false;

    [Inject]
    private void Construct(
        SignalBus signalBus,
        FreeLookCameraPosController freeLookCameraPosController,
        ArenaController arenaController,
        IChangeStateMenuSender menuStateSender
        )
    {
        _isControlOn = true;

        _signalBus = signalBus;
        _arenaController = arenaController;
        _menuStateSender = menuStateSender;

        _freeLookCameraPosController = freeLookCameraPosController;
        _arenaController.StopArenaBattle += OffAllControl;
        _menuStateSender.ChangeState += ChangeControlState;
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetPlayer);
        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetActions);

        if (_arenaController != null)
            _arenaController.StopArenaBattle -= OffAllControl;
        if (_menuStateSender != null)
            _menuStateSender.ChangeState -= ChangeControlState;

        if (_isActionsSet)
        {
            if (_buttonThirdPersonView != null)
            {
                _buttonThirdPersonView.OnThirdPersonView -= SetThirdPersonControl;
                _buttonThirdPersonView.OffThirdPersonView -= SetFreeLookViewControl;
            }
            if (_arenaStateToggle != null)
            {
                _arenaStateToggle.BattleStarted -= StartCombat;
                _arenaStateToggle.BattleStoped -= StopCombat;
            }
            if (_characterEventObserver != null)
            {
                _characterEventObserver.OnSetDefaultState -= SetDefaultState;
                _characterEventObserver.OnSetOffControlState -= OffAllControl;
                _characterEventObserver.OnDeath -= OffAllControl;
            }
        }
    }

    private void SetPlayer(PlayerSpawnedSignal args)
    {
        _buttonThirdPersonView = args.Player.gameObject.GetComponent<ButtonThirdPersonView>();

        _freeLookView = args.Player.gameObject.GetComponent<FreeLookViewScript>();
        _thirdPersonView = args.Player.gameObject.GetComponent<ThirdPersonViewScript>();
        _topDownView = args.Player.gameObject.GetComponent<TopDownViewScript>();

        _characterHit = args.Player.gameObject.GetComponent<PlayableCharacterHitScripts>();
        _arenaStateToggle = args.Player.gameObject.GetComponent<ButtonArenaStateToggle>();
        _characterEventObserver = args.Player.gameObject.GetComponent<PlayerEventObserver>();
    }

    private void SetActions()
    {
        _buttonThirdPersonView.OnThirdPersonView += SetThirdPersonControl;
        _buttonThirdPersonView.OffThirdPersonView += SetFreeLookViewControl;

        _arenaStateToggle.BattleStarted += StartCombat;
        _arenaStateToggle.BattleStoped += StopCombat;
        _characterEventObserver.OnSetDefaultState += SetDefaultState;
        _characterEventObserver.OnSetOffControlState += OffAllControl;
        _characterEventObserver.OnDeath += OffAllControl;

        _isActionsSet = true;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayerSpawnedSignal>(SetPlayer);
        _signalBus.Subscribe<PlayerSpawnedSignal>(SetActions);
    }

    private void ChangeControlState(bool isMenuOn)
    {
        if (isMenuOn)
        {
            _isControlOn = false;
            DisableView();
        }
        else
        {
            _isControlOn = true;
            EnableView();
        }
    }

    private void SetThirdPersonControl()
    {
        ChangeControl(CameraTypes.ThirdPersonView);
    }

    private void SetFreeLookViewControl()
    {
        ChangeControl(CameraTypes.FreeLookView);
    }

    private void StartCombat()
    {
        ChangeControl(CameraTypes.TopDownView);
    }

    private void StopCombat()
    {
        SetFreeLookViewControl();
    }

    private void ChangeControl(CameraTypes type)
    {
        _currentViewMode = type;
        OffAllControl();

        if (!_isControlOn)
            return;

        OnChoicedViewControl(type);
    }


    private void OnChoicedViewControl(CameraTypes type)
    {
        switch (type)
        {
            case CameraTypes.FreeLookView:
                FreeLookControlSettings();
                break;
            case CameraTypes.ThirdPersonView:
                ThirdPersonViewControlSettings();
                break;
            case CameraTypes.TopDownView:
                TopDownViewControlSettings();
                break;
            default:
                OffAllControl();
                break;
        }
    }

    private void TopDownViewControlSettings()
    {
        _topDownView.enabled = true;

        _characterHit.enabled = true;
    }

    private void ThirdPersonViewControlSettings()
    {
        _thirdPersonView.enabled = true;

        _buttonThirdPersonView.enabled = true;
        _freeLookCameraPosController.Enabled = true;
        _characterHit.enabled = true;
    }

    private void FreeLookControlSettings()
    {
        _freeLookView.enabled = true;

        _buttonThirdPersonView.enabled = true;
        _characterHit.enabled = true;
    }

    private void EnableView()
    {
        _currentViewControl = GetViewController(_currentViewMode);
        if (_currentViewControl != null)
        {
            _currentViewControl.Enabled = true;

            OnChoicedViewControl(_currentViewMode);
        }
    }

    private void DisableView()
    {
        _currentViewControl = GetViewController(_currentViewMode);
        if (_currentViewControl != null)
        {
            _currentViewControl.Enabled = false;

            DisableAdditionalScripts();
        }
    }

    private AbstractViewScript GetViewController(CameraTypes type)
    {
        return type switch
        {
            CameraTypes.FreeLookView => _freeLookView,
            CameraTypes.ThirdPersonView => _thirdPersonView,
            CameraTypes.TopDownView => _topDownView,
            _ => null,
        };
    }

    public void OffAllControl()
    {
        _freeLookView.enabled = false;
        _thirdPersonView.enabled = false;
        _topDownView.enabled = false;

        DisableAdditionalScripts();
    }

    private void DisableAdditionalScripts()
    {
        _buttonThirdPersonView.enabled = false;
        _freeLookCameraPosController.Enabled = false;
        _characterHit.enabled = false;
    }

    public void SetDefaultState()
    {
        SetFreeLookViewControl();
    }
}
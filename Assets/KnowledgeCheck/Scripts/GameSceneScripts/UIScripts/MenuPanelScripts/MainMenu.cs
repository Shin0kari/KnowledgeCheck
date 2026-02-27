using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour, IChangeStateMenuSender
{
    private PlayerInputSystem _playerInput;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private MainMenuUtils _menuUtils;

    public event Action<bool> ChangeState;

    private bool _isAvailableToSwitch;

    // public event Action OpenMenu;
    // public event Action CloseMenu;

    private void Awake()
    {
        _playerInput = new PlayerInputSystem();

        _isAvailableToSwitch = true;

        _playerInput.Player.Menu.performed += context => ChangeMenuActiveStatus();
        if (_menuUtils.IsStopGameOnMenu)
        {
            _playerInput.Player.Menu.performed += context => ChangeGameTimeScale();
        }
    }

    private void ChangeMenuActiveStatus()
    {
        if (!_isAvailableToSwitch)
            return;

        _mainMenu.SetActive(!_mainMenu.activeSelf);
        SendChangeMenuStatusSignal(_mainMenu.activeSelf);
    }

    private void SendChangeMenuStatusSignal(bool menuState)
    {
        if (menuState)
        {
            CursorVisibility.OnCursorVisibility();
        }
        else
        {
            CursorVisibility.OffCursorVisibility();
        }
        ChangeState?.Invoke(menuState);
    }

    public void ChangeMenuAvailableState(bool newState)
    {
        _isAvailableToSwitch = newState;
    }

    private void ChangeGameTimeScale()
    {
        if (_mainMenu.activeSelf)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        _playerInput?.Enable();
    }

    private void OnDisable()
    {
        _playerInput?.Disable();
    }
}

public interface IChangeStateMenuSender
{
    public event Action<bool> ChangeState;
}
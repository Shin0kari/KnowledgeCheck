using System;
using UnityEngine;

public class ButtonThirdPersonView : MonoBehaviour
{
    private PlayerInputSystem _playerInput;

    private bool _isPressed = false;
    private bool _prevPressed = false;

    public event Action OnThirdPersonView;
    public event Action OffThirdPersonView;

    private void Awake()
    {
        _playerInput = new PlayerInputSystem();

        _isPressed = false;
        _prevPressed = false;
        _playerInput.Enable();
    }

    void OnDestroy()
    {
        _playerInput.Disable();
    }

    private void Update()
    {
        _isPressed = _playerInput.Player.Aim.IsPressed();

        if (_isPressed != _prevPressed)
        {
            if (_isPressed)
            {
                OnThirdPersonView?.Invoke();
            }
            else
            {
                OffThirdPersonView?.Invoke();
            }

            _prevPressed = _isPressed;
        }
    }
}
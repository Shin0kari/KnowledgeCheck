using System;
using UnityEngine;
using Zenject;

public class ButtonArenaStateToggle : MonoBehaviour
{
    private PlayerInputSystem _playerInput;

    private bool _isPressed = false;
    private bool _prevPressed = false;
    private bool _isArenaStarted = false;

    public event Action BattleStarted;
    public event Action BattleStoped;

    private void OnEnable()
    {
        _playerInput = new PlayerInputSystem();
        _playerInput.Enable();

        _prevPressed = false;
        _isArenaStarted = false;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // При отжатии кнопки взаимодействия с ареной, у нас 
    // либо начинается Сражение на арене
    // либо останавливается Сражение на арене 
    private void Update()
    {
        _isPressed = _playerInput.Player.Interact.IsPressed();

        if (!_isPressed && _prevPressed)
        {
            if (!_isArenaStarted)
            {
                BattleStarted?.Invoke();
                _isArenaStarted = true;
            }
            // else
            // {
            //     BattleStoped?.Invoke();
            //     _isArenaStarted = false;
            // }

        }

        _prevPressed = _isPressed;
    }
}

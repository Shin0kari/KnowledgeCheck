using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public static class CursorVisibility
{
    private static Vector2 _oldCursorPos = new();
    private static bool _isAlwaysVisible = false;

    private static Mouse _currentMouse;

    static CursorVisibility()
    {
        _currentMouse = Mouse.current;
        SetDefaultCursorState();
    }

    public static void OnCursorVisibility()
    {
        SetActiveCursorState();
    }

    public static void OffCursorVisibility()
    {
        SetDefaultCursorState();
    }

    public static void OnAlwaysCursorVisibility()
    {
        _isAlwaysVisible = true;
        SetActiveCursorState();
    }

    public static void OffAlwaysCursorVisibility()
    {
        _isAlwaysVisible = false;
    }

    private static void SetDefaultCursorState()
    {
        if (_isAlwaysVisible)
            return;

        _oldCursorPos = _currentMouse.position.ReadValue();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void SetActiveCursorState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _currentMouse.WarpCursorPosition(_oldCursorPos);
    }
}
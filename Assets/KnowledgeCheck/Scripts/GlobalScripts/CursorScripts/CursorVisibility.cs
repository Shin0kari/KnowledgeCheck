using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public static class CursorVisibility
{
    private static Vector2 _oldCursorPos;
    private static bool _isAlwaysVisible = false;

    static CursorVisibility()
    {
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

        _oldCursorPos = Mouse.current.position.ReadValue();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void SetActiveCursorState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Mouse.current.WarpCursorPosition(_oldCursorPos);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ButtonRegistry : IButtonRegistry, IDisposable
{
    private readonly List<UIButton> _buttons = new();
    public event Action<UIButton> ButtonAdded;
    public event Action<UIButton> ButtonRemoved;

    public void Dispose()
    {
        _buttons.Clear();

        ButtonAdded = null;
        ButtonRemoved = null;
    }

    public void Register(UIButton button)
    {
        _buttons.Add(button);
        ButtonAdded?.Invoke(button);
    }

    public void Unregister(UIButton button)
    {
        _buttons.Remove(button);
        ButtonRemoved?.Invoke(button);
    }

    public List<UIButton> GetButtons() => _buttons;
}
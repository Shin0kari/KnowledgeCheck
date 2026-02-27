using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ButtonRegistry : IButtonRegistry
{
    private readonly List<UIButton> _buttons = new();
    public event Action<UIButton> ButtonAdded;
    public event Action<UIButton> ButtonRemoved;

    // [Inject]
    // private void Construct(IButton[] buttons)
    // {
    //     _buttons.AddRange(buttons);
    // }

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

    // public void Dispose()
    // {
    //     Debug.Log("[5_ButtonRegistry]: Disposable");
    // }

    // public void Initialize()
    // {
    //     Debug.Log("[5_ButtonRegistry]: Init");
    // }
}
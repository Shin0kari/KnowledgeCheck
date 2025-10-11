using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ButtonRegistry : IButtonRegistry
{
    private readonly List<IButton> _buttons = new();
    public event Action<IButton> ButtonAdded;
    public event Action<IButton> ButtonRemoved;

    [Inject]
    private void Construct(IButton[] buttons)
    {
        _buttons.AddRange(buttons);
    }

    public void Register(IButton button)
    {
        Debug.Log("[BUTTON_REGISTRY]: Start button registration.");
        _buttons.Add(button);
        ButtonAdded?.Invoke(button);
        Debug.Log("[BUTTON_REGISTRY]: Signal ButtonAdded is send.");
    }

    public void Unregister(IButton button)
    {
        _buttons.Remove(button);
        ButtonRemoved?.Invoke(button);
    }

    public List<IButton> GetButtons() => _buttons;
}
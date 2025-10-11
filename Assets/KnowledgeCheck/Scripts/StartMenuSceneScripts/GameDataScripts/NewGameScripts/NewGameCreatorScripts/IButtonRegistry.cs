using System;
using System.Collections.Generic;

public interface IButtonRegistry
{
    public event Action<IButton> ButtonAdded;
    public event Action<IButton> ButtonRemoved;

    public List<IButton> GetButtons();
    public void Register(IButton button);
    public void Unregister(IButton button);
}

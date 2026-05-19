using System;
using System.Collections.Generic;

public interface IButtonRegistry
{
    public event Action<UIButton> ButtonAdded;
    public event Action<UIButton> ButtonRemoved;

    public List<UIButton> GetButtons();
    public void Register(UIButton button);
    public void Unregister(UIButton button);
}

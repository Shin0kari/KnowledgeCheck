using UnityEngine;

public abstract class AbstractFactoryStarter
{
    protected bool _isFactoryActive = false;
    public virtual void Enable()
    {
        _isFactoryActive = true;
    }

    public virtual void Disable()
    {
        _isFactoryActive = false;
    }
}

using System;
using Zenject;

public class GlobalBoostrap : IDisposable
{
    // [Inject]
    // private void Construct() { }
    public void Dispose()
    {
        TypeCache.Dispose();
    }
}
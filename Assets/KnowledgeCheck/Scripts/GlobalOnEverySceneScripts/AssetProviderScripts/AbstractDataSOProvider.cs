using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public abstract class AbstractDataSOProvider : IDisposable
{
    protected CoreContextProvider _coreContextProvider;
    private ReactiveProperty<ScriptableObject> _reactiveDataSO;

    protected abstract Type DataType { get; }

    protected CancellationTokenSource _ct = new();

    [Inject]
    protected virtual void Construct(CoreContextProvider coreContextProvider)
    {
        _coreContextProvider = coreContextProvider;
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();
    }

    public virtual async UniTask<ReactiveProperty<ScriptableObject>> TryGetDataSO(CancellationToken ct)
    {
        if (_coreContextProvider == null)
            return null;

        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        while (_reactiveDataSO == null)
        {
            try
            {
                _reactiveDataSO = await _coreContextProvider.TryGetScriptableSettings(DataType, linkedCTS.Token);
                if (_reactiveDataSO == null) await UniTask.WaitForSeconds(0.1f, cancellationToken: linkedCTS.Token);
            }
            catch (System.OperationCanceledException)
            {
                return null;
            }
        }

        return _reactiveDataSO;
    }
}
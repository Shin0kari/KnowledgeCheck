using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Zenject;

public class EnemyPoolFiller : IInitializable, IDisposable
{
    private readonly Enemy.Pool _enemiesPool;
    private IEnemyPrefabStatusProvider _factory;

    private int _numEnemiesOnStart = 6;

    private DisposableBag _dB;
    private UniTaskCompletionSource _enemyPrefabLoadedSource = new();
    private CancellationTokenSource _ct = new();

    public EnemyPoolFiller(
        Enemy.Pool enemiesPool,
        IEnemyPrefabStatusProvider factory
    )
    {
        _enemiesPool = enemiesPool;
        _factory = factory;

        SubscribeOnUpdateObject();
    }

    public void Dispose()
    {
        _enemyPrefabLoadedSource?.TrySetCanceled();
        _enemyPrefabLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObject()
    {
        _factory.IsPrefabInit.Subscribe(isPrefabInit =>
        {
            if (isPrefabInit)
                _enemyPrefabLoadedSource.TrySetResult();
        }).AddTo(ref _dB);
    }

    public void Initialize()
    {
        AsyncExpandEnemyPool().Forget();
    }

    private async UniTask AsyncExpandEnemyPool()
    {
        await _enemyPrefabLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        _enemiesPool.ExpandBy(_numEnemiesOnStart);
    }
}
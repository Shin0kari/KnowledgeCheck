using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;
using System.Linq;
using System.Threading;
using R3;

public class EnemyPoolFactory : AbstractFactoryStarter, IInitializable, IDisposable
{
    private const int ENEMY_SPAWN_TIME_DELAY = 5000;

    private Enemy.Pool _enemiesPool;
    private List<Enemy> _enemies = new();

    private IAssetProviderGetter _assetProvider;
    private SignalBus _signalBus;
    private Player _player;

    // Вызывается из арены, поэтому и зависим от ArenaController
    private readonly ArenaController _arenaController;
    private ArenaUtils _arenaUtils;

    public event Action<Enemy> OnSpawnCharacter;

    private DisposableBag _dB;
    private UniTaskCompletionSource _arenaUtilsLoadedSource = new();
    private readonly CancellationTokenSource _ct = new();

    public EnemyPoolFactory(
        Enemy.Pool enemiesPool,
        IAssetProviderGetter assetProvider,
        SignalBus signalBus,
        ArenaController arenaController
    )
    {
        _enemiesPool = enemiesPool;
        _assetProvider = assetProvider;
        _signalBus = signalBus;
        _arenaController = arenaController;

        SubscribeOnUpdateObjects();
        _signalBus.Subscribe<PlayerSpawnedSignal>(SetPlayer);
    }

    public void Dispose()
    {
        if (_arenaController != null)
        {
            _arenaController.StartSpawnEnemy -= EnableSpawnSystem;
            _arenaController.StopSpawnEnemy -= DisableSpawnSystem;
        }

        _signalBus?.TryUnsubscribe<PlayerSpawnedSignal>(SetPlayer);

        _arenaUtilsLoadedSource.TrySetCanceled();
        _arenaUtilsLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();

        _enemies.Clear();
        OnSpawnCharacter = null;
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ArenaUtils>()
            .OfType<IBindingSingletonComponent, ArenaUtils>()
            .Subscribe(arenaUtils =>
            {
                if (arenaUtils == null)
                    return;

                _arenaUtils = arenaUtils;
                _arenaUtilsLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void SetPlayer(PlayerSpawnedSignal args)
    {
        _player = args.Player;
    }

    public void Initialize()
    {
        _arenaController.StartSpawnEnemy += EnableSpawnSystem;
        _arenaController.StopSpawnEnemy += DisableSpawnSystem;
        // _arenaController.StopArenaBattle += DisableSpawnSystem;
    }

    private void EnableSpawnSystem()
    {
        if (_isFactoryActive)
            return;
        Enable();

        EnemyAsyncSpawner().Forget();
    }

    private async UniTaskVoid EnemyAsyncSpawner()
    {
        await _arenaUtilsLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        while (_isFactoryActive)
        {
            while (_enemies.Count < _arenaUtils.EnemiesCount)
            {
                if (!_isFactoryActive || _ct.IsCancellationRequested)
                    return;

                var enemy = _enemiesPool.Spawn(_player);
                enemy.gameObject.SetActive(true);
                _enemies.Add(enemy);

                SetDeathSignalSubscribe(enemy);

                OnSpawnCharacter?.Invoke(enemy);
                await UniTask.Yield(cancellationToken: _ct.Token);
            }

            await UniTask.Delay(ENEMY_SPAWN_TIME_DELAY, cancellationToken: _ct.Token);
        }
    }

    private void SetDeathSignalSubscribe(Enemy enemy)
    {
        enemy.Killed += DeathSignalHandler;
    }

    private void DeathSignalHandler(Enemy enemy)
    {
        enemy.Killed -= DeathSignalHandler;
        DespawnEnemy(enemy);
    }

    private void DespawnEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        // Можно добавить таймер для enemy, чтобы по истечении которого, полностью деспавнился враг
        // _enemiesPool.Despawn(enemy);
    }

    private void DisableSpawnSystem()
    {
        Disable();
    }
}

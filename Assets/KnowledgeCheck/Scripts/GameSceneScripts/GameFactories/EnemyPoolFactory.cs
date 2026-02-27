using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;
using System.Linq;
using System.Threading;

public class EnemyPoolFactory : AbstractFactoryStarter, IInitializable, IDisposable
{
    private const int ENEMY_SPAWN_TIME_DELAY = 5000;

    private Enemy.Pool _enemiesPool;
    private List<Enemy> _enemies = new();

    private SignalBus _signalBus;
    private Player _player;

    // Вызывается из арены, поэтому и зависим от ArenaController
    private readonly ArenaController _arenaController;
    private ArenaUtils _arenaUtils;

    private readonly CancellationTokenSource _token = new();

    public event Action<Enemy> OnSpawnCharacter;

    public EnemyPoolFactory(
        Enemy.Pool enemiesPool,
        SignalBus signalBus,
        ArenaController arenaController,
        ArenaUtils arenaUtils
    )
    {
        _enemiesPool = enemiesPool;
        _signalBus = signalBus;
        _arenaController = arenaController;
        _arenaUtils = arenaUtils;

        _signalBus.Subscribe<PlayerSpawnedSignal>(SetPlayer);
    }

    public void Dispose()
    {
        _enemies = null;
        OnSpawnCharacter = null;

        _signalBus.Unsubscribe<PlayerSpawnedSignal>(SetPlayer);

        if (_arenaController != null)
        {
            _arenaController.StartSpawnEnemy -= EnableSpawnSystem;
            _arenaController.StopSpawnEnemy -= DisableSpawnSystem;
        }

        _token?.Cancel();
        _token?.Dispose();
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
        while (_isFactoryActive)
        {
            while (_enemies.Count < _arenaUtils.EnemiesCount)
            {
                if (!_isFactoryActive || _token.IsCancellationRequested)
                    return;

                var enemy = _enemiesPool.Spawn(_player);
                _enemies.Add(enemy);

                SetDeathSignalSubscribe(enemy);

                OnSpawnCharacter?.Invoke(enemy);
            }

            await UniTask.Delay(ENEMY_SPAWN_TIME_DELAY, cancellationToken: _token.Token);
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

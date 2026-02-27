using System;
using UnityEngine;
using Zenject;

public class CharactersObserver : IDisposable
{
    private readonly Enemy.Pool _enemiesPool;
    private EnemyPoolFactory _enemyPoolFactory;
    private SignalBus _signalBus;
    private HealthBarFactory _healthBarFactory;

    [Inject]
    public CharactersObserver(
        Enemy.Pool enemiesPool,
        EnemyPoolFactory enemyPoolFactory,
        SignalBus signalBus,
        HealthBarFactory healthBarFactory)
    {
        _enemiesPool = enemiesPool;
        _enemyPoolFactory = enemyPoolFactory;
        _signalBus = signalBus;
        _healthBarFactory = healthBarFactory;

        _enemyPoolFactory.OnSpawnCharacter += SetActionsOnEnemySpawn;
        _signalBus.Subscribe<PlayerSpawnedSignal>(SetActionOnPlayerSpawn);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<PlayerSpawnedSignal>(SetActionOnPlayerSpawn);
        if (_enemyPoolFactory != null)
            _enemyPoolFactory.OnSpawnCharacter -= SetActionsOnEnemySpawn;
    }

    private void SetActionsOnEnemySpawn(Enemy enemy)
    {
        _healthBarFactory.SpawnNotPlayableCharacterHealthBar(enemy);
    }

    private void SetActionOnPlayerSpawn(PlayerSpawnedSignal args)
    {
        _healthBarFactory.SpawnPlayableCharacterHealthBar(args.Player);
    }
}
using System;
using UnityEngine;
using Zenject;

public class CharactersObserver : IDisposable
{
    private EnemyPoolFactory _enemyPoolFactory;
    private SignalBus _signalBus;
    private HealthBarFactory _healthBarFactory;

    public CharactersObserver(
        EnemyPoolFactory enemyPoolFactory,
        SignalBus signalBus,
        HealthBarFactory healthBarFactory)
    {
        _enemyPoolFactory = enemyPoolFactory;
        _signalBus = signalBus;
        _healthBarFactory = healthBarFactory;

        _enemyPoolFactory.OnSpawnCharacter += SetActionsOnEnemySpawn;
        _signalBus.Subscribe<PlayerSpawnedSignal>(SetActionOnPlayerSpawn);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<PlayerSpawnedSignal>(SetActionOnPlayerSpawn);
        if (_enemyPoolFactory != null)
            _enemyPoolFactory.OnSpawnCharacter -= SetActionsOnEnemySpawn;
    }

    private void SetActionsOnEnemySpawn(Enemy enemy)
    {
        _healthBarFactory.AsyncSpawnNotPlayableCharacterHealthBar(enemy).Forget();
    }

    private void SetActionOnPlayerSpawn(PlayerSpawnedSignal args)
    {
        _healthBarFactory.SpawnPlayableCharacterHealthBarAsync(args.Player).Forget();
    }
}
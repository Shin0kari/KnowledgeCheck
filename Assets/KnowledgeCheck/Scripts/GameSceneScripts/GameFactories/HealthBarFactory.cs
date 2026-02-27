using UnityEngine;
using Zenject;

public class HealthBarFactory : AbstractFactoryStarter
{
    private GameObject _enemyCanvasPrefab;
    private GameObject _playerCanvasPrefab;

    private HealthBar.Factory _healthBarFactory;

    [Inject]
    private void Construct(
        GameObject enemyCanvasPrefab,
        GameObject playerCanvasPrefab,
        HealthBar.Factory healthBarFactory
    )
    {
        _enemyCanvasPrefab = enemyCanvasPrefab;
        _playerCanvasPrefab = playerCanvasPrefab;
        _healthBarFactory = healthBarFactory;
    }

    public void SpawnNotPlayableCharacterHealthBar(Enemy enemy)
    {
        var enemyHealthBar = _healthBarFactory.Create(_enemyCanvasPrefab);
        enemyHealthBar.transform.SetParent(enemy.transform);
        enemyHealthBar.transform.position = new(enemy.transform.position.x, enemyHealthBar.transform.position.y, enemy.transform.position.z);

        enemyHealthBar.SetDamagableObject(enemy);
    }

    public void SpawnPlayableCharacterHealthBar(Player player)
    {
        var playerHealthBar = _healthBarFactory.Create(_playerCanvasPrefab);

        playerHealthBar.SetDamagableObject(player);
    }
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterEventObserver))]
public class Enemy : MonoBehaviour, INotPlayableCharacter, IDamagable, IDisposable
{
    private const float LOWER_HEALTH_VALUE_RANGE = 0f;

    [SerializeField] private float _enemySpawnMaxDistance = 10f;
    [SerializeField] private float _enemySpawnMinDistance = 8f;
    [SerializeField] private float _spawnHeight = 3f;
    [SerializeField] private float _maxHealthValue = 100f;

    private CharacterData _character = new();

    private SignalBus _signalBus;
    private Player _player;

    private ArenaUtils _arenaUtils;
    private CharacterEventObserver _characterEventObserver;

    private CancellationTokenSource _cts;

    public CharacterEventObserver CharacterEventObserver { get { return _characterEventObserver; } }

    public event Action<Enemy> Spawned;
    public event Action<Enemy> Killed;
    public event Action<float> HealthChanged;

    [Inject]
    private void Construct(ArenaUtils arenaUtils)
    {
        _arenaUtils = arenaUtils;

        _characterEventObserver = GetComponent<CharacterEventObserver>();
        _characterEventObserver.OnDeath += SendDeathSignal;
    }

    public void Dispose()
    {
        if (_characterEventObserver != null)
            _characterEventObserver.OnDeath -= SendDeathSignal;

        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetSignalPlayer);

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        Spawned = null;
        Killed = null;
        HealthChanged = null;
    }

    private void SendDeathSignal()
    {
        Killed?.Invoke(this);
    }

    private async UniTaskVoid ResetEnemyAsync(CancellationToken cts)
    {
        if (_player == null)
        {
            return;
        }

        await ResetEnemyDataAsync(cts);
        gameObject.SetActive(true);
        _characterEventObserver.SetSpawnState();
    }

    private async UniTask ResetEnemyDataAsync(CancellationToken cts)
    {
        await ResetEnemyPos(cts);
        ResetEnemyDir();
        SetDefaultEnemyData();
        ResetEnemyHealth();

        Spawned?.Invoke(this);
    }

    private async UniTask ResetEnemyPos(CancellationToken cts)
    {
        gameObject.transform.position = await GenerateRandomEnemyPos(cts);
    }

    private async UniTask<Vector3> GenerateRandomEnemyPos(CancellationToken cts)
    {
        Vector3 randomEnemyPos = new();
        bool isPosAvailable = false;
        while (!isPosAvailable)
        {
            if (_player == null)
            {
                isPosAvailable = true;
                continue;
            }


            var randomDirFromPlayer = GenerateRandomDirection();
            var randomEnemySpawnDistance = GenerateRandomDistance();

            randomEnemyPos = GeneratePosition(randomDirFromPlayer, randomEnemySpawnDistance);

            isPosAvailable = CheckEnemySpawnPosAvailability(randomEnemyPos);
            await UniTask.Yield(cancellationToken: cts);
        }
        return randomEnemyPos;
    }

    private float GenerateRandomDirection()
    {
        return UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
    }

    private float GenerateRandomDistance()
    {
        return UnityEngine.Random.Range(_enemySpawnMinDistance, _enemySpawnMaxDistance);
    }

    private Vector3 GeneratePosition(float randomDirFromPlayer, float randomEnemySpawnDistance)
    {
        var targetPos = _player.transform.position;
        // var targetPos = _playerProvider.Player.transform.position;

        var enemyPosX = targetPos.x + Mathf.Sin(randomDirFromPlayer) * randomEnemySpawnDistance;
        var enemyPosZ = targetPos.z + Mathf.Cos(randomDirFromPlayer) * randomEnemySpawnDistance;

        return new(enemyPosX, _arenaUtils.PosY + _spawnHeight, enemyPosZ);
    }

    private bool CheckEnemySpawnPosAvailability(Vector3 randomEnemyPos)
    {
        if (_arenaUtils.MaxPosX < randomEnemyPos.x || _arenaUtils.MinPosX > randomEnemyPos.x)
            return false;
        if (_arenaUtils.MaxPosZ < randomEnemyPos.z || _arenaUtils.MinPosZ > randomEnemyPos.z)
            return false;
        return true;
    }

    private void ResetEnemyDir()
    {
        Vector3 targetPosition = _player.transform.position;
        Vector3 enemyPosition = gameObject.transform.position;

        Vector3 direction = (targetPosition - enemyPosition).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ResetEnemyHealth()
    {
        // _character.Stats.Health = _maxHealthValue;
        HealthChanged?.Invoke(_character.Stats.Health);
    }

    public float GetHealth()
    {
        return _character.Stats.Health;
    }

    public void ChangeHealth(float value)
    {
        _character.Stats.Health -= value;

        if (_character.Stats.Health > _maxHealthValue)
            _character.Stats.Health = _maxHealthValue;
        if (_character.Stats.Health <= LOWER_HEALTH_VALUE_RANGE)
        {
            _character.Stats.Health = LOWER_HEALTH_VALUE_RANGE;

            _characterEventObserver.SetDeathState();
        }

        HealthChanged?.Invoke(_character.Stats.Health);
    }

    public Player GetEnemyTarget()
    {
        return _player;
    }

    private void SetDefaultEnemyData()
    {
        CharacterStats characterStats = new()
        {
            Health = 100f,
            Damage = 10f,
            Defense = 0f,
        };

        CharacterAffects characterAffects = new()
        {
            Speed = 1f,
            Regeneration = 0f
        };

        _character = new()
        {
            Pos = transform.position,
            Direction = transform.rotation,
            Inventory = null,
            Stats = characterStats,
            Affects = characterAffects
        };
    }

    private void SetPlayer(Player player)
    {
        if (player != null)
            _player = player;
        else
        {
            _signalBus.Subscribe<PlayerSpawnedSignal>(SetSignalPlayer);
        }
    }

    private void SetSignalPlayer(PlayerSpawnedSignal args)
    {
        _player = args.Player;
    }

    public void OnSpawned(Player player)
    {
        _cts = new();
        SetPlayer(player);
        ResetEnemyAsync(_cts.Token).Forget();
    }

    public void OnDespawned()
    {
        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetSignalPlayer);

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        Spawned = null;
        Killed = null;
        HealthChanged = null;
    }

    public class Pool : MonoMemoryPool<Player, Enemy>
    {
        protected override void OnSpawned(Enemy enemy) { }
        protected override void Reinitialize(Player player, Enemy enemy)
        {
            enemy.OnSpawned(player);
        }
        protected override void OnDespawned(Enemy enemy)
        {
            enemy.OnDespawned();
            base.OnDespawned(enemy);
        }
    }
}

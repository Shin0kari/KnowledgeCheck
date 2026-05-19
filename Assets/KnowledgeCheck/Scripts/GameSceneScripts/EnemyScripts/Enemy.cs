using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterEventObserver))]
public class Enemy : MonoBehaviour, INotPlayableCharacter, IDamagable
{
    private const float LOWER_HEALTH_VALUE_RANGE = 0f;

    [SerializeField] private float _enemySpawnMaxDistance = 10f;
    [SerializeField] private float _enemySpawnMinDistance = 8f;
    [SerializeField] private float _spawnHeight = 4f;
    [SerializeField] private float _maxHealthValue = 100f;

    private IAssetProviderGetter _assetProvider;
    private CharacterData _character = new();
    private CharacterType _characterType;
    private string _characterName;

    private SignalBus _signalBus;
    private Player _player;

    private ArenaUtils _arenaUtils;
    private CharacterEventObserver _characterEventObserver;

    private float _randomDirFromPlayer;
    private float _randomEnemySpawnDistance;
    private Vector3 _randomEnemyPos;
    private bool _isPosAvailable;

    private Vector3 _targetPosition;
    private Vector3 _enemyPosition;

    public CharacterEventObserver CharacterEventObserver { get { return _characterEventObserver; } }

    public event Action<Enemy> Spawned;
    public event Action<Enemy> Killed;
    public event Action<float> HealthChanged;

    private DisposableBag _dB;
    private UniTaskCompletionSource _arenaUtilsLoadedSource = new();
    private CancellationTokenSource _cts;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        SubscribeOnUpdateObjects();

        _characterEventObserver = GetComponent<CharacterEventObserver>();
        _characterEventObserver.OnDeath += SendDeathSignal;
    }

    private void OnDestroy()
    {
        if (_characterEventObserver != null)
            _characterEventObserver.OnDeath -= SendDeathSignal;

        _signalBus?.TryUnsubscribe<PlayerSpawnedSignal>(SetSignalPlayer);

        _arenaUtilsLoadedSource.TrySetCanceled();
        _arenaUtilsLoadedSource = null;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        _dB.Dispose();


        ClearActions();
    }

    public void ClearActions()
    {
        Spawned = null;
        Killed = null;
        HealthChanged = null;
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

        await _arenaUtilsLoadedSource.Task.AttachExternalCancellation(cts);
        await ResetEnemyDataAsync(cts);
        gameObject.SetActive(true);
        _characterEventObserver.SetSpawnState();
    }

    private async UniTask ResetEnemyDataAsync(CancellationToken cts)
    {
        await ResetEnemyPos(cts);
        ResetEnemyDir();
        // SetDefaultEnemyData();
        ResetEnemyHealth();

        Spawned?.Invoke(this);
    }

    private async UniTask ResetEnemyPos(CancellationToken cts)
    {
        gameObject.transform.position = await GenerateRandomEnemyPos(cts);
    }

    private async UniTask<Vector3> GenerateRandomEnemyPos(CancellationToken cts)
    {
        _isPosAvailable = false;
        _randomEnemyPos = new();
        while (!_isPosAvailable)
        {
            if (_player == null)
            {
                _isPosAvailable = true;
                await UniTask.Yield(cancellationToken: cts);
                continue;
            }

            _randomDirFromPlayer = GenerateRandomDirection();
            _randomEnemySpawnDistance = GenerateRandomDistance();

            GeneratePosition(_randomDirFromPlayer, _randomEnemySpawnDistance, ref _randomEnemyPos);

            _isPosAvailable = CheckEnemySpawnPosAvailability(_randomEnemyPos);
            await UniTask.Yield(cancellationToken: cts);
        }
        return _randomEnemyPos;
    }

    private float GenerateRandomDirection()
    {
        return UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
    }

    private float GenerateRandomDistance()
    {
        return UnityEngine.Random.Range(_enemySpawnMinDistance, _enemySpawnMaxDistance);
    }

    private void GeneratePosition(in float randomDirFromPlayer, in float randomEnemySpawnDistance, ref Vector3 _newEnemyPos)
    {
        var targetPos = _player.transform.position;
        // var targetPos = _playerProvider.Player.transform.position;

        var enemyPosX = targetPos.x + Mathf.Sin(randomDirFromPlayer) * randomEnemySpawnDistance;
        var enemyPosZ = targetPos.z + Mathf.Cos(randomDirFromPlayer) * randomEnemySpawnDistance;

        _newEnemyPos.x = enemyPosX;
        _newEnemyPos.y = _arenaUtils.PosY + _spawnHeight;
        _newEnemyPos.z = enemyPosZ;
    }

    private bool CheckEnemySpawnPosAvailability(in Vector3 randomEnemyPos)
    {
        if (_arenaUtils.MaxPosX < randomEnemyPos.x || _arenaUtils.MinPosX > randomEnemyPos.x)
            return false;
        if (_arenaUtils.MaxPosZ < randomEnemyPos.z || _arenaUtils.MinPosZ > randomEnemyPos.z)
            return false;
        return true;
    }

    private void ResetEnemyDir()
    {
        _targetPosition = _player.transform.position;
        _enemyPosition = gameObject.transform.position;

        Vector3 direction = (_targetPosition - _enemyPosition).normalized;
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

    public void ChangeHealth(in float value)
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

    public CharacterType GetCharacterType()
    {
        return _characterType;
    }

    public string GetCharacterName()
    {
        return _characterName;
    }

    public void SetCharacterData(
        CharacterStats characterStats,
        CharacterAffects characterAffects,
        Inventory characterInventory,
        CharacterType characterType,
        string characterName)
    {
        _character = new()
        {
            Pos = transform.position,
            Direction = transform.rotation,
            Inventory = characterInventory,
            Stats = characterStats,
            Affects = characterAffects
        };
        _characterType = characterType;
        _characterName = characterName;
    }

    // private void SetDefaultEnemyData()
    // {
    //     CharacterStats characterStats = new()
    //     {
    //         Health = 100f,
    //         Damage = 10f,
    //         Defense = 0f,
    //     };

    //     CharacterAffects characterAffects = new()
    //     {
    //         Speed = 1f,
    //         Regeneration = 0f
    //     };

    //     _character = new()
    //     {
    //         Pos = transform.position,
    //         Direction = transform.rotation,
    //         Inventory = null,
    //         Stats = characterStats,
    //         Affects = characterAffects
    //     };
    // }

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
        _signalBus?.TryUnsubscribe<PlayerSpawnedSignal>(SetSignalPlayer);

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

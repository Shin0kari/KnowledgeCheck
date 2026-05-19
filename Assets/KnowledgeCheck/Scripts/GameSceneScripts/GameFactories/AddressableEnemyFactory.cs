using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class AddressableEnemyFactory : IFactory<Enemy>, IEnemyPrefabStatusProvider, IDisposable
{
    private readonly DiContainer _container;
    private IAddressablesProvider _aP;
    private SceneCharactersSettingsRepository _sceneCharactersSettingsRepository;
    private SceneCharactersSettingsSO _sceneCharactersSettingsSO;

    private ReactiveProperty<BaseCharacterSettingsSO> _reactiveEnemyCharacterSettingsSO;
    private BaseCharacterSettingsSO _enemySettingsSO;
    private GameObject _enemyPrefab;

    private bool _isPrefabInit = false;
    private readonly ReactiveProperty<bool> _reactiveIsPrefabInit = new();
    public ReadOnlyReactiveProperty<bool> IsPrefabInit => _reactiveIsPrefabInit;

    private DisposableBag _dB;
    private UniTaskCompletionSource _enemySettingsSOLoadedSource = new();
    private CancellationTokenSource _ct = new();


    public AddressableEnemyFactory(
        DiContainer container,
        IAddressablesProvider aP,
        SceneCharactersSettingsRepository sceneCharactersSettingsRepository
    )
    {
        _container = container;
        _sceneCharactersSettingsRepository = sceneCharactersSettingsRepository;
        _aP = aP;

        AsyncLoadResource().Forget();
        AsyncSetEnemyPrefab().Forget();
    }

    public void Dispose()
    {
        DisposeTokens();
        DisposeDynamicProperties();

    }

    private void DisposeTokens()
    {
        _enemySettingsSOLoadedSource.TrySetCanceled();
        _enemySettingsSOLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private void DisposeDynamicProperties()
    {
        IsPrefabInit.Dispose();
        _reactiveIsPrefabInit.Dispose();
    }

    private async UniTask AsyncLoadResource()
    {
        _sceneCharactersSettingsSO = await _sceneCharactersSettingsRepository.AsyncGetSceneCharactersSettingsSO(_ct.Token);

        await LoadCharacterSettingsSOResources<SkeletonSettingsSO>(_sceneCharactersSettingsSO);
    }

    private async UniTask LoadCharacterSettingsSOResources<T>(SceneCharactersSettingsSO sceneCharactersSettingsSO) where T : BaseCharacterSettingsSO
    {
        _reactiveEnemyCharacterSettingsSO = await sceneCharactersSettingsSO.GetEnemySettings(typeof(T), _ct.Token);

        _reactiveEnemyCharacterSettingsSO?
            .OfType<BaseCharacterSettingsSO, T>()
            .Subscribe(enemySettingsSO =>
            {
                if (enemySettingsSO == null)
                    return;

                _enemySettingsSO = enemySettingsSO;
                _enemySettingsSOLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    public async UniTask AsyncSetEnemyPrefab()
    {
        await _enemySettingsSOLoadedSource.Task.AttachExternalCancellation(_ct.Token);
        _enemyPrefab = await _aP.AsyncGetAddressablesDataFromReference<GameObject>(_enemySettingsSO.CharacterPrefab, _ct.Token);
        _isPrefabInit = true;
        _reactiveIsPrefabInit.Value = _isPrefabInit;
    }

    private void SetDefaultEnemyData(Enemy enemy, BaseCharacterSettingsSO settingsSO)
    {
        CharacterStats characterStats = settingsSO.CharacterBaseData.Stats with { };
        CharacterAffects characterAffects = settingsSO.CharacterBaseData.Affects with { };
        Inventory characterInventory = settingsSO.CharacterBaseData.Inventory with { };

        enemy.SetCharacterData(
            characterStats,
            characterAffects,
            characterInventory,
            settingsSO.CharacterType,
            settingsSO.CharacterName);
    }

    public Enemy Create()
    {
        if (!_isPrefabInit)
            ErrorMessageGenerator.GenerateSimpleError(this, "Enemys prefabs not init");

        _enemyPrefab.SetActive(false);
        var enemy = _container.InstantiatePrefabForComponent<Enemy>(_enemyPrefab);
        SetDefaultEnemyData(enemy, _enemySettingsSO);

        return enemy;
    }
}

public interface IEnemyPrefabStatusProvider
{
    ReadOnlyReactiveProperty<bool> IsPrefabInit { get; }
}
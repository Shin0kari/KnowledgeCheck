using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "SceneCharactersSettingsSO", menuName = "Scene Context/Scene Characters Settings/All Scene Characters Settings SO", order = 0)]
public class SceneCharactersSettingsSO : ScriptableObject
{
    // public AssetReferenceT<BaseCharacterSettingsSO>[] ArrayPlayersCharacterSettingsSO { get { return _arrayPlayersCharacterSettingsSO; } }
    [SerializeField] private List<AssetReferenceT<BaseCharacterSettingsSO>> _arrayPlayersCharacterSettingsSO = new();
    private List<AsyncOperationHandle<BaseCharacterSettingsSO>> _referenceArrayPlayerCharacterSettingsSO = new();
    private Dictionary<Type, ReactiveProperty<BaseCharacterSettingsSO>> _loadedPlayerCharacterConfigs = new();

    // public AssetReferenceT<BaseCharacterSettingsSO>[] ArrayEnemiesCharacterSettingsSO { get { return _arrayEnemiesCharacterSettingsSO; } }
    [SerializeField] private List<AssetReferenceT<BaseCharacterSettingsSO>> _arrayEnemiesCharacterSettingsSO = new();
    private List<AsyncOperationHandle<BaseCharacterSettingsSO>> _referenceArrayEnemyCharacterSettingsSO = new();
    private Dictionary<Type, ReactiveProperty<BaseCharacterSettingsSO>> _loadedEnemyCharacterConfigs = new();

    private UniTaskCompletionSource _playerConfigsLoadedSource = new();
    private UniTaskCompletionSource _enemyConfigsLoadedSource = new();
    private CancellationTokenSource _ct = new();

    public async UniTask LoadAllConfigs(CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );
        try
        {
            List<UniTask> tasks = new() {
                LoadPlayerCharacterSettings(linkedCTS.Token),
                LoadEnemyCharacterSettings(linkedCTS.Token),
            };

            await UniTask.WhenAll(tasks).AttachExternalCancellation(linkedCTS.Token);
        }
        catch (System.Exception err)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, err);
        }
    }

    public void DisposeSO()
    {
        ClearAllTokens();
        UnloadAllAddressables();
        ClearReactiveProperties();
    }

    public void SetDefaultState()
    {
        DisposeSO();
        SetNewTokens();
    }

    private void SetNewTokens()
    {
        _playerConfigsLoadedSource = new();
        _enemyConfigsLoadedSource = new();
        _ct = new();
    }

    private async UniTask LoadPlayerCharacterSettings(CancellationToken ct)
    {
        _loadedPlayerCharacterConfigs = await LoadEachPlayerConfigs(ct);
        Debug.Log($"Player Num loaded configs: {_loadedPlayerCharacterConfigs.Count}");

        _playerConfigsLoadedSource.TrySetResult();
    }

    private async UniTask LoadEnemyCharacterSettings(CancellationToken ct)
    {
        _loadedEnemyCharacterConfigs = await LoadEachEnemyConfigs(ct);
        Debug.Log($"Enemy Num loaded configs: {_loadedEnemyCharacterConfigs.Count}");

        _enemyConfigsLoadedSource.TrySetResult();
    }

    private async UniTask<Dictionary<Type, ReactiveProperty<BaseCharacterSettingsSO>>> LoadEachPlayerConfigs(CancellationToken ct)
    {
        Dictionary<Type, ReactiveProperty<BaseCharacterSettingsSO>> newLoadedConfigs = new();

        BaseCharacterSettingsSO[] loadResults = await LoadAllPlayerSOConfigs(ct);

        foreach (var configSO in loadResults)
        {
            if (configSO == null) continue;

            ReactiveProperty<BaseCharacterSettingsSO> newReactiveProperty = new();
            newLoadedConfigs.Add(configSO.GetType(), newReactiveProperty);

            newReactiveProperty.Value = configSO;
        }
        return newLoadedConfigs;
    }

    private async UniTask<Dictionary<Type, ReactiveProperty<BaseCharacterSettingsSO>>> LoadEachEnemyConfigs(CancellationToken ct)
    {
        Dictionary<Type, ReactiveProperty<BaseCharacterSettingsSO>> newLoadedConfigs = new();

        BaseCharacterSettingsSO[] loadResults = await LoadAllEnemySOConfigs(ct);

        foreach (var configSO in loadResults)
        {
            if (configSO == null) continue;

            ReactiveProperty<BaseCharacterSettingsSO> newReactiveProperty = new();
            newLoadedConfigs.Add(configSO.GetType(), newReactiveProperty);

            newReactiveProperty.Value = configSO;
        }
        return newLoadedConfigs;
    }

    private async UniTask<BaseCharacterSettingsSO[]> LoadAllPlayerSOConfigs(CancellationToken ct)
    {
        var loadTasks = new List<UniTask<BaseCharacterSettingsSO>>();
        _referenceArrayPlayerCharacterSettingsSO = new(_arrayPlayersCharacterSettingsSO.Count());

        foreach (var referenceSO in _arrayPlayersCharacterSettingsSO)
        {
            var reference = referenceSO.LoadAssetAsync<BaseCharacterSettingsSO>();
            _referenceArrayPlayerCharacterSettingsSO.Add(reference);
            loadTasks.Add(reference.ToUniTask(cancellationToken: ct));
        }

        return await UniTask.WhenAll(loadTasks).AttachExternalCancellation(ct);
    }

    private async UniTask<BaseCharacterSettingsSO[]> LoadAllEnemySOConfigs(CancellationToken ct)
    {
        var loadTasks = new List<UniTask<BaseCharacterSettingsSO>>();
        _referenceArrayEnemyCharacterSettingsSO = new(_arrayEnemiesCharacterSettingsSO.Count());

        foreach (var referenceSO in _arrayEnemiesCharacterSettingsSO)
        {
            var reference = referenceSO.LoadAssetAsync<BaseCharacterSettingsSO>();
            _referenceArrayEnemyCharacterSettingsSO.Add(reference);
            loadTasks.Add(reference.ToUniTask(cancellationToken: ct));
        }

        return await UniTask.WhenAll(loadTasks).AttachExternalCancellation(ct);
    }

    public async UniTask<ReactiveProperty<BaseCharacterSettingsSO>> GetPlayerSettings(Type awaitedParameterType, CancellationToken externalCt)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            externalCt
        );
        try
        {
            await _playerConfigsLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }

        if (!_loadedPlayerCharacterConfigs.TryGetValue(awaitedParameterType, out var configSO))
        {
            ErrorMessageGenerator.GenerateErrorMessage(this, $"Config {awaitedParameterType} not found", out string errMessage);
            Debug.LogError(errMessage);
        }

        return configSO;
    }

    public async UniTask<ReactiveProperty<BaseCharacterSettingsSO>> GetEnemySettings(Type awaitedParameterType, CancellationToken externalCt)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            externalCt
        );
        try
        {
            await _enemyConfigsLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }

        if (!_loadedEnemyCharacterConfigs.TryGetValue(awaitedParameterType, out var configSO))
        {
            ErrorMessageGenerator.GenerateErrorMessage(this, $"Config {awaitedParameterType} not found", out string errMessage);
            Debug.LogError(errMessage);
        }

        return configSO;
    }

    private void ClearAllTokens()
    {
        _playerConfigsLoadedSource.TrySetCanceled();
        _playerConfigsLoadedSource = null;

        _enemyConfigsLoadedSource.TrySetCanceled();
        _enemyConfigsLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();
        _ct = null;
    }

    private void UnloadAllAddressables()
    {
        if (_referenceArrayEnemyCharacterSettingsSO != null)
        {
            foreach (var reference in _referenceArrayEnemyCharacterSettingsSO) reference.Release();
            _referenceArrayEnemyCharacterSettingsSO.Clear();
        }
        if (_referenceArrayPlayerCharacterSettingsSO != null)
        {
            foreach (var reference in _referenceArrayPlayerCharacterSettingsSO) reference.Release();
            _referenceArrayPlayerCharacterSettingsSO.Clear();
        }

    }

    private void ClearReactiveProperties()
    {
        if (_loadedPlayerCharacterConfigs != null)
        {
            foreach (var reactiveConfig in _loadedPlayerCharacterConfigs.Values)
            {
                reactiveConfig?.Dispose();
            }
            _loadedPlayerCharacterConfigs.Clear();
        }
        if (_loadedEnemyCharacterConfigs != null)
        {
            foreach (var reactiveConfig in _loadedEnemyCharacterConfigs.Values)
            {
                reactiveConfig?.Dispose();
            }
            _loadedEnemyCharacterConfigs.Clear();
        }
        foreach (var playersCharacterSettingsSO in _referenceArrayPlayerCharacterSettingsSO)
        {
            playersCharacterSettingsSO.Release();
        }
        foreach (var enemiesCharacterSettingsSO in _referenceArrayEnemyCharacterSettingsSO)
        {
            enemiesCharacterSettingsSO.Release();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "CoreContextSO", menuName = "Scene Context/Scene Core Context SO")]
public class CoreContextSO : ScriptableObject
{
    [SerializeField] private AssetReferenceT<ScriptableObject>[] _arraySceneSO;
    private List<AsyncOperationHandle<ScriptableObject>> _referenceArraySceneSO = new();
    private Dictionary<Type, ReactiveProperty<ScriptableObject>> _loadedConfigs = new();

    private UniTaskCompletionSource _allConfigsLoadedSource = new();
    private CancellationTokenSource _ct = new();

    public async UniTask LoadAllConfigs(CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        SetNewCompletionSource(ref _allConfigsLoadedSource);

        await LoadEachConfigs(linkedCTS.Token);
        Debug.Log($"[{GetType().ToString().ToUpper()}]: Num loaded configs: {_loadedConfigs.Count}");

        _allConfigsLoadedSource.TrySetResult();
        try
        {
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
        catch (System.Exception err)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, err);
        }
    }

    private void OnDestroy()
    {
        ClearAllTokens();
        UnloadAllAddressables();
        ClearReactiveProperties();
    }

    private void ClearReactiveProperties()
    {
        foreach (var reactiveConfig in _loadedConfigs.Values)
        {
            reactiveConfig?.Dispose();
        }
        _loadedConfigs.Clear();
        foreach (var sceneSO in _referenceArraySceneSO)
        {
            sceneSO.Release();
        }
    }

    private async UniTask LoadEachConfigs(CancellationToken ct)
    {
        ClearReactiveProperties();

        ScriptableObject[] loadResults = await LoadAllSOConfigs(ct);

        foreach (var configSO in loadResults)
        {
            if (configSO != null)
            {
                ReactiveProperty<ScriptableObject> newReactiveProperty = new();
                _loadedConfigs.Add(configSO.GetType(), newReactiveProperty);

                newReactiveProperty.Value = configSO;
                await UniTask.Yield(ct);
            }
        }
    }

    private async UniTask<ScriptableObject[]> LoadAllSOConfigs(CancellationToken ct)
    {
        var loadTasks = new List<UniTask<ScriptableObject>>();
        _referenceArraySceneSO = new(_arraySceneSO.Count());

        foreach (var referenceSO in _arraySceneSO)
        {
            var reference = referenceSO.LoadAssetAsync<ScriptableObject>();
            _referenceArraySceneSO.Add(reference);
            loadTasks.Add(reference.ToUniTask(cancellationToken: ct));
        }

        return await UniTask.WhenAll(loadTasks).AttachExternalCancellation(ct);
    }

    private void SetNewCompletionSource(ref UniTaskCompletionSource tcs)
    {
        tcs?.TrySetCanceled();
        var newTcs = new UniTaskCompletionSource();
        tcs = newTcs;
    }

    public async UniTask<ReactiveProperty<ScriptableObject>> GetSceneConfig(Type awaitedParameterType, CancellationToken externalCt)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            externalCt
        );

        try
        {
            await _allConfigsLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);
            if (!_loadedConfigs.TryGetValue(awaitedParameterType, out var configSO))
            {
                ErrorMessageGenerator.GenerateErrorMessage(this, $"Config {awaitedParameterType} not found", out string errMessage);
            }

            return configSO;
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }

    }

    private void ClearAllTokens()
    {
        _allConfigsLoadedSource.TrySetCanceled();
        _allConfigsLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();
    }

    private void UnloadAllAddressables()
    {
        if (_referenceArraySceneSO != null)
            foreach (var reference in _referenceArraySceneSO) reference.Release();
        foreach (var reactiveSO in _loadedConfigs.Values)
        {
            reactiveSO?.Dispose();
        }
        _loadedConfigs.Clear();
    }
}
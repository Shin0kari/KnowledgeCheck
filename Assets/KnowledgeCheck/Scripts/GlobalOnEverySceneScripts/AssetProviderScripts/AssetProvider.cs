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
using Zenject;
using ObservableCollections;

public class AssetProvider : IAssetProviderSpawner, IAssetProviderGetter, IDisposable
{
    private DiContainer _container;
    private IAddressablesProvider _aP;

    private Dictionary<Type, ReactiveProperty<IBindingSingletonComponent>> _awaitedSingletonProperties = new();
    private Dictionary<Type, ObservableList<IBindingTransientComponent>> _awaitedTransientProperties = new();
    // private Dictionary<Type, ReactiveProperty<List<IBindingTransientComponent>>> _awaitedTransientProperties = new();
    private Dictionary<AssetReference, ReactiveProperty<UnityEngine.Object>> _loadedResource = new();

    private List<GameObject> _spawnedGOInstance;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        DiContainer container,
        IAddressablesProvider aP)
    {
        _container = container;
        _aP = aP;
    }

    public void Dispose()
    {
        ClearProperties();
        ClearResources();

        _ct?.Cancel();
        _ct?.Dispose();
    }

    public async UniTask<List<GameObject>> SpawnPrefabAssets(List<AssetReferenceT<GameObject>> spawnableAssets, Transform parentTransform, DiContainer contextContainer, CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        _spawnedGOInstance = new(spawnableAssets.Count);
        List<UniTask> tasks = new(spawnableAssets.Count);

        foreach (var assetReference in spawnableAssets)
        {
            GameObject addressableInstance = await LoadPrefab(assetReference, linkedCTS.Token);

            GameObject instance = InstantiatePrefab(parentTransform, addressableInstance, linkedCTS.Token);

            tasks.Add(CheckComponentsOnWaiting(instance, linkedCTS.Token));

            _spawnedGOInstance.Add(instance);
        }

        await UniTask.WhenAll(tasks).AttachExternalCancellation(linkedCTS.Token);
        return _spawnedGOInstance;
        // try
        // {
        // }
        // catch (Exception error)
        // {
        //     ErrorMessageGenerator.GenerateSimpleError(this, error);
        //     return null;
        // }
    }

    private async UniTask CheckComponentsOnWaiting(GameObject instance, CancellationToken ct)
    {
        var bindingSingletonComponents = instance.GetComponentsInChildren<IBindingSingletonComponent>(includeInactive: true);
        var bindingTransientComponents = instance.GetComponentsInChildren<IBindingTransientComponent>(includeInactive: true);

        List<UniTask> tasks = new(bindingSingletonComponents.Count() + bindingTransientComponents.Count());

        foreach (var component in bindingSingletonComponents)
        {
            tasks.Add(CheckEntireComponentOnWaiting(component, ct));
        }

        foreach (var component in bindingTransientComponents)
        {
            tasks.Add(CheckEntireComponentOnWaiting(component, ct));
        }

        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct);
    }

    private async UniTask CheckEntireComponentOnWaiting(IBindingSingletonComponent component, CancellationToken ct)
    {
        Type[] relevantTypes = TypeCache.GetRelatedTypes(component.GetType());

        foreach (var type in relevantTypes)
        {
            // Если синглтон был найден и он не пуст, то он будет переписан
            var reactiveComponent = TryCheckIBindingSingletonType(type);
            if (reactiveComponent != null)
            {
                reactiveComponent.Value = component;
            }
            await UniTask.Yield(ct);
        }
    }

    private async UniTask CheckEntireComponentOnWaiting(IBindingTransientComponent component, CancellationToken ct)
    {
        Type[] relevantTypes = TypeCache.GetRelatedTypes(component.GetType());

        foreach (var type in relevantTypes)
        {
            // Если множественное значение было найдено, то оно будет дополнено
            var reactiveComponent = TryCheckIBindingTransientType(type);
            reactiveComponent?.Add(component);

            await UniTask.Yield(ct);
        }
    }

    private GameObject InstantiatePrefab(Transform parentTransform, GameObject addressableInstance, CancellationToken ct)
    {
        GameObject instance = _container.InstantiatePrefab(addressableInstance, parentTransform);
        return instance;
    }

    private UniTask<GameObject> LoadPrefab(AssetReferenceT<GameObject> assetReference, CancellationToken ct)
    {
        var task = _aP.AsyncGetAddressablesDataFromReference<GameObject>(assetReference, ct);

        return task;
    }

    public ReactiveProperty<IBindingSingletonComponent> GetIBindingSingletonComponent<T>() where T : IBindingSingletonComponent
    {
        var type = typeof(T);
        return GetIBindingSingleton(type);
    }

    public ObservableList<IBindingTransientComponent> GetIBindingTransientComponent<T>() where T : IBindingTransientComponent
    {
        var type = typeof(T);
        return GetIBindingTransient(type);
    }

    // public ReactiveProperty<List<IBindingTransientComponent>> GetIBindingTransientComponent<T>() where T : IBindingTransientComponent
    // {
    //     var type = typeof(T);
    //     return GetIBindingTransient(type);
    // }

    private ReactiveProperty<IBindingSingletonComponent> TryCheckIBindingSingletonType(Type checkedType)
    {
        if (!typeof(IBindingSingletonComponent).IsAssignableFrom(checkedType)) return null;
        return GetIBindingSingleton(checkedType);
    }

    private ObservableList<IBindingTransientComponent> TryCheckIBindingTransientType(Type checkedType)
    {
        if (!typeof(IBindingTransientComponent).IsAssignableFrom(checkedType)) return null;
        return GetIBindingTransient(checkedType);
    }

    // private ReactiveProperty<List<IBindingTransientComponent>> TryCheckIBindingTransientType(Type checkedType)
    // {
    //     if (!typeof(IBindingTransientComponent).IsAssignableFrom(checkedType)) return null;
    //     return GetIBindingTransient(checkedType);
    // }

    private ReactiveProperty<IBindingSingletonComponent> GetIBindingSingleton(Type type)
    {
        if (_awaitedSingletonProperties.TryGetValue(type, out var reactiveProperty))
        {
            return reactiveProperty;
        }

        ReactiveProperty<IBindingSingletonComponent> newReactiveProperty = new();
        _awaitedSingletonProperties.Add(type, newReactiveProperty);

        if (TryCheckContainer(type, out IBindingSingletonComponent foundedComponent))
        {
            newReactiveProperty.Value = foundedComponent;
        }

        return newReactiveProperty;
    }

    private ObservableList<IBindingTransientComponent> GetIBindingTransient(Type checkedType)
    {
        if (_awaitedTransientProperties.TryGetValue(checkedType, out var reactiveProperty))
        {
            return reactiveProperty;
        }

        ObservableList<IBindingTransientComponent> newReactiveProperty = new();
        _awaitedTransientProperties.Add(checkedType, newReactiveProperty);

        return newReactiveProperty;
    }

    // private ReactiveProperty<List<IBindingTransientComponent>> GetIBindingTransient(Type checkedType)
    // {
    //     if (_awaitedTransientProperties.TryGetValue(checkedType, out var reactiveProperty))
    //     {
    //         return reactiveProperty;
    //     }

    //     ReactiveProperty<List<IBindingTransientComponent>> newReactiveProperty = new() { Value = new() };
    //     _awaitedTransientProperties.Add(checkedType, newReactiveProperty);

    //     return newReactiveProperty;
    // }

    private bool TryCheckContainer(Type awaitedParameterType, out IBindingSingletonComponent founderComponent)
    {
        if (_container.HasBinding(awaitedParameterType))
        {
            var resolved = _container.TryResolve(awaitedParameterType);

            if (resolved is IBindingSingletonComponent component)
            {
                founderComponent = component;
                return true;
            }
        }

        founderComponent = null;
        return false;
    }

    public async UniTask<ReactiveProperty<UnityEngine.Object>> GetSharedResourceData<T>(AssetReferenceT<T> resourceReference, CancellationToken ct) where T : UnityEngine.Object
    {
        if (_loadedResource.TryGetValue(resourceReference, out var resource))
        {
            return resource;
        }

        try
        {
            ReactiveProperty<UnityEngine.Object> newReactiveProperty = new();
            _loadedResource.Add(resourceReference, newReactiveProperty);

            await LoadReferenceResource(resourceReference, newReactiveProperty, _ct.Token, ct);

            return newReactiveProperty;
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }

    }

    private async UniTask LoadReferenceResource<T>(
        AssetReferenceT<T> resourceReference,
        ReactiveProperty<UnityEngine.Object> newReactiveProperty,
        CancellationToken ct1,
        CancellationToken ct2
    ) where T : UnityEngine.Object
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(ct1, ct2);
        var value = await _aP.AsyncGetAddressablesDataFromReference<T>(resourceReference, linkedCTS.Token);
        newReactiveProperty.Value = value;
    }

    private void ClearProperties()
    {
        _spawnedGOInstance?.Clear();

        ClearSingletonProperties();
        ClearTransientProperties();
    }

    private void ClearSingletonProperties()
    {
        foreach (var reactiveProperty in _awaitedSingletonProperties.Values)
        {
            reactiveProperty.Dispose();
        }
        _awaitedSingletonProperties?.Clear();
    }

    private void ClearTransientProperties()
    {
        foreach (var reactivePropertyList in _awaitedTransientProperties.Values)
        {
            reactivePropertyList.Clear();
        }
        _awaitedTransientProperties?.Clear();
    }

    private void ClearResources()
    {
        foreach (var reactiveResource in _loadedResource.Values)
        {
            reactiveResource.Dispose();
        }

        _loadedResource?.Clear();
    }
}

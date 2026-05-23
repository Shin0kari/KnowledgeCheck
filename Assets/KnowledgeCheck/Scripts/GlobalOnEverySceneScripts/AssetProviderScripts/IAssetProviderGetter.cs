using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IAssetProviderGetter
{
    public ReactiveProperty<IBindingSingletonComponent> GetIBindingSingletonComponent<T>() where T : IBindingSingletonComponent;
    public ObservableList<IBindingTransientComponent> GetIBindingTransientComponent<T>() where T : IBindingTransientComponent;
    // public ReactiveProperty<List<IBindingTransientComponent>> GetIBindingTransientComponent<T>() where T : IBindingTransientComponent;
    public UniTask<ReactiveProperty<UnityEngine.Object>> GetSharedResourceData<T>(AssetReferenceT<T> resourceReference, CancellationToken ct) where T : UnityEngine.Object;
}
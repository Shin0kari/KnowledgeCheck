using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class CoreContextProvider : IDisposable
{
    private AssetReferenceT<CoreContextSO> _coreContext;
    private CoreContextSO _coreContextSO;

    private IAssetProviderGetter _assetProviderGetter;
    private ReactiveProperty<UnityEngine.Object> _coreReactiveProperty;

    private DisposableBag _dB;

    private UniTaskCompletionSource _coreContextSOLoadedSource = new();

    private CancellationTokenSource _ctSO = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        AssetReferenceT<CoreContextSO> coreContext,
        IAssetProviderGetter assetProviderGetter
    )
    {
        _coreContext = coreContext;
        _assetProviderGetter = assetProviderGetter;

        LoadCoreContext().Forget();
    }

    public void Dispose()
    {
        _coreContextSOLoadedSource.TrySetCanceled();
        _coreContextSOLoadedSource = null;

        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private async UniTask LoadCoreContext()
    {
        if (_assetProviderGetter == null)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "AssetProvider not set");
            throw new System.Exception();
        }

        _coreReactiveProperty = await _assetProviderGetter.GetSharedResourceData(_coreContext, _ct.Token);

        _coreReactiveProperty?
            .Subscribe(property =>
            {
                if (property == null)
                    return;
                SetCoreContext(property).Forget();
            })
            .AddTo(ref _dB);
    }

    private async UniTask SetCoreContext(UnityEngine.Object property)
    {
        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _ctSO = new();

        if (property is not CoreContextSO contextSO)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Property is not CoreContextSO");
            return;
        }

        _coreContextSO = contextSO;
        await AsyncLoadAllConfigs(_coreContextSO, _ctSO.Token);
        _coreContextSOLoadedSource.TrySetResult();
    }

    private async UniTask AsyncLoadAllConfigs(CoreContextSO context, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );
        await context.LoadAllConfigs(linkedCTS.Token);
    }

    public async UniTask<ReactiveProperty<ScriptableObject>> TryGetScriptableSettings(Type awaitedParameterType, CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, ct);

        try
        {
            await _coreContextSOLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);
            var reactiveProperty = await _coreContextSO.GetSceneConfig(awaitedParameterType, linkedCTS.Token);
            return reactiveProperty;
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }
    }
}
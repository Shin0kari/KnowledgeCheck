using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class AudioService : IAudioService, IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private CoreGlobalAudioProvider _coreGlobalAudioProvider;

    private ReactiveProperty<ScriptableObject> _reactiveGlobalAudio;
    private GlobalAudioSO _globalAudioSO;

    private ReadOnlyReactiveProperty<AudioClip> _reactiveClickSound;
    private ReadOnlyReactiveProperty<AudioClip> _reactiveClickPanelSound;
    private AudioClip _clickSound;
    private AudioClip _clickPanelSound;

    private AudioSource _sceneAudioSource;

    private DisposableBag _dB;
    private DisposableBag _iDB;
    private CancellationTokenSource _ctSO = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        CoreGlobalAudioProvider coreGlobalAudioProvider
    )
    {
        _assetProvider = assetProvider;
        _coreGlobalAudioProvider = coreGlobalAudioProvider;

        LoadGlobalAudio().Forget();
    }

    public void Dispose()
    {
        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _ct?.Cancel();
        _ct?.Dispose();

        _iDB.Dispose();
        _dB.Dispose();
    }

    private async UniTaskVoid LoadGlobalAudio()
    {
        try
        {
            _reactiveGlobalAudio = await _coreGlobalAudioProvider.TryGetDataSO(_ct.Token);

            _reactiveGlobalAudio?
                .Subscribe(globalAudioSO =>
                {
                    if (globalAudioSO == null)
                        return;

                    SetSO(globalAudioSO).Forget();
                })
                .AddTo(ref _dB);

        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    private async UniTask SetSO(ScriptableObject globalAudioSO)
    {
        _ctSO?.Cancel();
        _ctSO?.Dispose();
        _iDB.Dispose();

        _ctSO = new();
        _iDB = new();

        if (globalAudioSO is not GlobalAudioSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _globalAudioSO = so;

        await LoadSOResources(_globalAudioSO, _ctSO.Token);
    }

    private async UniTask LoadSOResources(GlobalAudioSO globalAudioSO, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        _reactiveClickSound = await GetReactiveClip(globalAudioSO.SimpleClickSound, linkedCTS.Token);
        _reactiveClickPanelSound = await GetReactiveClip(globalAudioSO.ClickPanelSound, linkedCTS.Token);

        _reactiveClickSound?
            .Subscribe((clickSound) =>
            {
                if (clickSound == null) return;
                _clickSound = clickSound;
            }).AddTo(ref _iDB);

        _reactiveClickPanelSound?
            .Subscribe((clickSound) =>
            {
                if (clickSound == null) return;
                _clickPanelSound = clickSound;
            }).AddTo(ref _iDB);
    }

    private async UniTask<ReadOnlyReactiveProperty<AudioClip>> GetReactiveClip(AssetReferenceT<AudioClip> clip, CancellationToken ct)
    {
        if (_assetProvider == null || clip == null)
            return null;

        try
        {
            return (await _assetProvider.GetSharedResourceData(clip, ct))
                    .OfType<UnityEngine.Object, AudioClip>()
                    .ToReadOnlyReactiveProperty();
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }
    }

    public void ChangeSceneAudioSource(AudioSource sceneAudioSource)
    {
        _sceneAudioSource = sceneAudioSource;
    }

    public void OnUIClick()
    {
        if (_sceneAudioSource == null || _clickSound == null)
            return;
        _sceneAudioSource.PlayOneShot(_clickSound);
    }

    public void OnUIClickButtonPanel()
    {
        if (_sceneAudioSource == null || _clickPanelSound == null)
            return;
        _sceneAudioSource.PlayOneShot(_clickPanelSound);
    }
}
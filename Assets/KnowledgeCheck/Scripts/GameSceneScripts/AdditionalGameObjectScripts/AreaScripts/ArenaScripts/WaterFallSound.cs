using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class WaterFallSound : MonoBehaviour
{
    private IAssetProviderGetter _assetProvider;
    private IAddressablesProvider _aP;
    private SceneAudioProvider _sceneAudioProvider;

    private ReactiveProperty<ScriptableObject> _reactiveSceneAudioSO;
    private SceneAudioSO _sceneAudioSO;

    private ReadOnlyReactiveProperty<SceneAmbientSoundsSO> _reactiveSceneAmbientSoundsSO;
    private SceneAmbientSoundsSO _sceneAmbientSoundsSO;

    private List<AudioClip> _waterfallSounds;

    private AudioSource _source;

    private UniTaskCompletionSource _sceneAmbientSoundsSOLoadedSource = new();
    private UniTaskCompletionSource _waterfallSoundsLoadedSource = new();

    private DisposableBag _dB;
    private DisposableBag _iDB;
    private CancellationTokenSource _ctSO = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        IAddressablesProvider aP,
        SceneAudioProvider SceneAudioProvider
    )
    {
        _assetProvider = assetProvider;
        _aP = aP;
        _sceneAudioProvider = SceneAudioProvider;

        _ct = gameObject.GetCancellationTokenOnDestroy();
        _source = GetComponent<AudioSource>();

        AsyncLoadResource().Forget();
        AsyncUploadSounds().Forget();
    }

    private void OnDestroy()
    {
        _sceneAmbientSoundsSOLoadedSource.TrySetCanceled();
        _sceneAmbientSoundsSOLoadedSource = null;

        _waterfallSoundsLoadedSource.TrySetCanceled();
        _waterfallSoundsLoadedSource = null;

        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _iDB.Dispose();
        _dB.Dispose();

        _waterfallSounds.Clear();
    }

    private void Start()
    {
        AsyncPlayWaterFallSound().Forget();
    }

    private async UniTask AsyncLoadResource()
    {
        _reactiveSceneAudioSO = await _sceneAudioProvider.TryGetDataSO(_ct);

        _reactiveSceneAudioSO?
            .Subscribe(SceneAudioSO =>
            {
                if (SceneAudioSO == null)
                    return;

                SetSO(SceneAudioSO).Forget();
            })
            .AddTo(ref _dB);
    }

    private async UniTask SetSO(ScriptableObject SceneAudioSO)
    {
        _ctSO?.Cancel();
        _ctSO?.Dispose();
        _iDB.Dispose();

        _ctSO = new();
        _iDB = new();

        if (SceneAudioSO is not SceneAudioSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _sceneAudioSO = so;
        await LoadSceneAmbientSoundsSOResources(_sceneAudioSO, _ctSO.Token);
    }

    private async UniTask LoadSceneAmbientSoundsSOResources(SceneAudioSO sceneAudioSO, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct,
            ct
        );

        _reactiveSceneAmbientSoundsSO = await GetReactiveScriptableObject(sceneAudioSO.SceneAmbientSoundsSO, linkedCTS.Token);

        _reactiveSceneAmbientSoundsSO?
            .Subscribe(sceneAmbientSoundsSO =>
            {
                if (sceneAmbientSoundsSO == null) return;

                _sceneAmbientSoundsSO = sceneAmbientSoundsSO;
                _sceneAmbientSoundsSOLoadedSource.TrySetResult();
            }).AddTo(ref _iDB);
    }

    private async UniTask<ReadOnlyReactiveProperty<SceneAmbientSoundsSO>> GetReactiveScriptableObject(AssetReferenceT<SceneAmbientSoundsSO> so, CancellationToken ct)
    {
        if (_assetProvider == null || so == null)
            return null;

        return
            (await _assetProvider.GetSharedResourceData(so, ct))
                .OfType<UnityEngine.Object, SceneAmbientSoundsSO>()
                .ToReadOnlyReactiveProperty();
    }

    private async UniTaskVoid AsyncUploadSounds()
    {
        await LoadAmbientSounds();
    }

    private async UniTask LoadAmbientSounds()
    {
        _waterfallSounds = await AsyncLoadWaterfallSounds();
        _waterfallSoundsLoadedSource.TrySetResult();
    }

    private async UniTask<List<AudioClip>> AsyncLoadWaterfallSounds()
    {
        await _sceneAmbientSoundsSOLoadedSource.Task.AttachExternalCancellation(_ct);
        return await ExtractSoundsFromAudioList(_sceneAmbientSoundsSO.WaterSounds.WaterFallSounds);
    }

    private async UniTask<List<AudioClip>> ExtractSoundsFromAudioList(AudioList audioList)
    {
        if (audioList == null)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Ambient StoneDoor sounds not found");
            return null;
        }

        var referenceAudioClips = audioList.GetClipsReference();

        List<AudioClip> loadedAudioClips = new(referenceAudioClips.Count);

        foreach (var referenceAudioClip in referenceAudioClips)
        {
            loadedAudioClips.Add(await _aP.AsyncGetAddressablesDataFromReference<AudioClip>(referenceAudioClip, _ct));
        }
        return loadedAudioClips;
    }

    private async UniTask AsyncPlayWaterFallSound()
    {
        await _waterfallSoundsLoadedSource.Task.AttachExternalCancellation(_ct);

        var choicedAudioClip = GetRandomizedSound(_waterfallSounds, out float pitch);

        SetMusicAndPlay(choicedAudioClip, pitch);
    }

    private AudioClip GetRandomizedSound(List<AudioClip> audioClips, out float pitch)
    {
        pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        if (audioClips.Count < 1) return null;

        int index = UnityEngine.Random.Range(0, audioClips.Count - 1);
        return audioClips[index];
    }

    private void SetMusicAndPlay(AudioClip music, float pitch)
    {
        if (_source == null || music == null)
            return;

        _source.pitch = pitch;
        _source.clip = music;
        _source.Play();
    }
}
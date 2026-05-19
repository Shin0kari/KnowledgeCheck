using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class TorchSound : MonoBehaviour
{
    private IAssetProviderGetter _assetProvider;
    private IAddressablesProvider _aP;
    private SceneAudioProvider _sceneAudioProvider;

    private ReactiveProperty<ScriptableObject> _reactiveSceneAudioSO;
    private SceneAudioSO _sceneAudioSO;

    private ReadOnlyReactiveProperty<SceneAmbientSoundsSO> _reactiveSceneAmbientSoundsSO;
    private SceneAmbientSoundsSO _sceneAmbientSoundsSO;

    private List<AudioClip> _fireSounds;

    private AudioSource _source;

    private UniTaskCompletionSource _sceneAmbientSoundsSOLoadedSource = new();
    private UniTaskCompletionSource _fireSoundsLoadedSource = new();

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

        _fireSoundsLoadedSource.TrySetCanceled();
        _fireSoundsLoadedSource = null;

        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _iDB.Dispose();
        _dB.Dispose();

        _fireSounds.Clear();
    }

    private void Start()
    {
        AsyncPlayFireSound().Forget();
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
        _fireSounds = await AsyncLoadFireSounds();
        _fireSoundsLoadedSource.TrySetResult();
    }

    private async UniTask<List<AudioClip>> AsyncLoadFireSounds()
    {
        await _sceneAmbientSoundsSOLoadedSource.Task.AttachExternalCancellation(_ct);
        return await ExtractSoundsFromAudioList(_sceneAmbientSoundsSO.FireSounds.SmallFireSounds);
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

    private async UniTask AsyncPlayFireSound()
    {
        await _fireSoundsLoadedSource.Task.AttachExternalCancellation(_ct);

        var choicedAudioClip = GetRandomizedSound(_fireSounds);

        SetMusicAndPlay(choicedAudioClip);
    }

    private AudioClip GetRandomizedSound(List<AudioClip> audioClips)
    {
        if (audioClips.Count < 1) return null;

        int index = UnityEngine.Random.Range(0, audioClips.Count - 1);
        return audioClips[index];
    }

    private void SetMusicAndPlay(AudioClip music)
    {
        if (_source == null || music == null)
            return;

        _source.clip = music;
        _source.loop = true;
        _source.Play();
    }
}
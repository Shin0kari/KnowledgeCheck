using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class GateSound : MonoBehaviour
{
    private IAssetProviderGetter _assetProvider;
    private IAddressablesProvider _aP;
    private SceneAudioProvider _sceneAudioProvider;

    private ReactiveProperty<ScriptableObject> _reactiveSceneAudioSO;
    private SceneAudioSO _sceneAudioSO;

    private ReadOnlyReactiveProperty<SceneInteractionsSoundsSO> _reactiveSceneInteractionsSoundsSO;
    private SceneInteractionsSoundsSO _sceneInteractionsSoundsSO;

    private List<AudioClip> _openGateSounds;
    private List<AudioClip> _closeGateSounds;

    private AudioSource _source;

    private UniTaskCompletionSource _sceneInteractionsSoundsSOLoadedSource = new();

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
        _sceneInteractionsSoundsSOLoadedSource.TrySetCanceled();
        _sceneInteractionsSoundsSOLoadedSource = null;

        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _iDB.Dispose();
        _dB.Dispose();

        _openGateSounds.Clear();
        _closeGateSounds.Clear();
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
        await LoadSceneInteractionsSoundsSOResources(_sceneAudioSO, _ctSO.Token);
    }

    private async UniTask LoadSceneInteractionsSoundsSOResources(SceneAudioSO sceneAudioSO, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct,
            ct
        );

        _reactiveSceneInteractionsSoundsSO = await GetReactiveScriptableObject(sceneAudioSO.SceneInteractionsSoundsSO, linkedCTS.Token);

        _reactiveSceneInteractionsSoundsSO?
            .Subscribe(sceneInteractionsSoundsSO =>
            {
                if (sceneInteractionsSoundsSO == null) return;

                _sceneInteractionsSoundsSO = sceneInteractionsSoundsSO;
                _sceneInteractionsSoundsSOLoadedSource.TrySetResult();
            }).AddTo(ref _iDB);
    }

    private async UniTask<ReadOnlyReactiveProperty<SceneInteractionsSoundsSO>> GetReactiveScriptableObject(AssetReferenceT<SceneInteractionsSoundsSO> so, CancellationToken ct)
    {
        if (_assetProvider == null || so == null)
            return null;

        return
            (await _assetProvider.GetSharedResourceData(so, ct))
                .OfType<UnityEngine.Object, SceneInteractionsSoundsSO>()
                .ToReadOnlyReactiveProperty();
    }

    private async UniTaskVoid AsyncUploadSounds()
    {
        await LoadAmbientSounds();
    }

    private async UniTask LoadAmbientSounds()
    {
        _openGateSounds = await AsyncLoadOpenGateSounds();
        _closeGateSounds = await AsyncLoadCloseGateSounds();
    }

    private async UniTask<List<AudioClip>> AsyncLoadOpenGateSounds()
    {
        await _sceneInteractionsSoundsSOLoadedSource.Task.AttachExternalCancellation(_ct);
        return await ExtractSoundsFromAudioList(_sceneInteractionsSoundsSO.StoneDoorAudio.OpenStoneDoorAudio);
    }

    private async UniTask<List<AudioClip>> AsyncLoadCloseGateSounds()
    {
        await _sceneInteractionsSoundsSOLoadedSource.Task.AttachExternalCancellation(_ct);
        return await ExtractSoundsFromAudioList(_sceneInteractionsSoundsSO.StoneDoorAudio.CloseStoneDoorAudio);
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

    public void PlayOpenGateSound()
    {
        AsyncPlayOpenGateSound().Forget();
    }
    public void PlayCloseGateSound()
    {
        AsyncPlayCloseGateSound().Forget();
    }

    private async UniTask AsyncPlayOpenGateSound()
    {
        await _sceneInteractionsSoundsSOLoadedSource.Task.AttachExternalCancellation(_ct);

        var choicedAudioClip = GetRandomizedSound(_openGateSounds, out float pitch);

        SetMusicAndPlay(choicedAudioClip, pitch);
    }

    private async UniTask AsyncPlayCloseGateSound()
    {
        await _sceneInteractionsSoundsSOLoadedSource.Task.AttachExternalCancellation(_ct);

        var choicedAudioClip = GetRandomizedSound(_closeGateSounds, out float pitch);

        SetMusicAndPlay(choicedAudioClip, pitch);
    }

    private AudioClip GetRandomizedSound(List<AudioClip> audioClips, out float pitch)
    {
        pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        if (audioClips == null || audioClips.Count < 1) return null;

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
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private IAssetProviderGetter _assetProvider;
    private IAddressablesProvider _aP;
    private SceneAudioProvider _sceneAudioProvider;

    private ReactiveProperty<ScriptableObject> _reactiveSceneAudioSO;
    private SceneAudioSO _sceneAudioSO;

    private ReadOnlyReactiveProperty<SceneMusicsSO> _reactiveSceneMusicsSO;
    private SceneMusicsSO _sceneMusicsSO;

    private List<AudioClip> _startGameMusics = new();
    private List<AudioClip> _endGameMusics = new();
    private ArenaController _arenaController;

    private AudioSource _source;

    private UniTaskCompletionSource _ambientSoundsLoadedSource = new();
    private UniTaskCompletionSource _sceneMusicsSOLoadedSource = new();

    private DisposableBag _dB;
    private DisposableBag _iDB;
    private CancellationTokenSource _ctSO = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        IAddressablesProvider aP,
        SceneAudioProvider SceneAudioProvider,
        ArenaController arenaController
    )
    {
        _assetProvider = assetProvider;
        _aP = aP;
        _sceneAudioProvider = SceneAudioProvider;
        _arenaController = arenaController;

        _ct = gameObject.GetCancellationTokenOnDestroy();
        _source = GetComponent<AudioSource>();

        AsyncLoadResource().Forget();
        AsyncUploadSounds().Forget();

        _arenaController.StopSpawnEnemy += StartPlayEndGameMusic;
    }

    private void OnDestroy()
    {
        if (_arenaController != null)
            _arenaController.StopSpawnEnemy -= StartPlayEndGameMusic;

        _ambientSoundsLoadedSource.TrySetCanceled();
        _ambientSoundsLoadedSource = null;

        _sceneMusicsSOLoadedSource.TrySetCanceled();
        _sceneMusicsSOLoadedSource = null;

        _ctSO?.Cancel();
        _ctSO?.Dispose();

        _iDB.Dispose();
        _dB.Dispose();

        _startGameMusics.Clear();
        _endGameMusics.Clear();
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
        await LoadSceneMusicsSOResources(_sceneAudioSO, _ctSO.Token);
    }

    private async UniTask LoadSceneMusicsSOResources(SceneAudioSO sceneAudioSO, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct,
            ct
        );

        _reactiveSceneMusicsSO = await GetReactiveScriptableObject(sceneAudioSO.SceneMusicsSO, linkedCTS.Token);

        _reactiveSceneMusicsSO?
            .Subscribe(sceneMusicsSO =>
            {
                if (sceneMusicsSO == null) return;

                _sceneMusicsSO = sceneMusicsSO;
                _sceneMusicsSOLoadedSource.TrySetResult();
            }).AddTo(ref _iDB);
    }

    private async UniTask<ReadOnlyReactiveProperty<SceneMusicsSO>> GetReactiveScriptableObject(AssetReferenceT<SceneMusicsSO> so, CancellationToken ct)
    {
        if (_assetProvider == null || so == null)
            return null;

        return
            (await _assetProvider.GetSharedResourceData(so, ct))
                .OfType<UnityEngine.Object, SceneMusicsSO>()
                .ToReadOnlyReactiveProperty();
    }

    private async UniTaskVoid AsyncUploadSounds()
    {
        await LoadAmbientSounds();
    }

    private async UniTask LoadAmbientSounds()
    {
        _startGameMusics = await AsyncLoadStartGameMusic();
        _endGameMusics = await AsyncLoadEndGameMusic();
        _ambientSoundsLoadedSource.TrySetResult();
    }

    private async UniTask<List<AudioClip>> AsyncLoadStartGameMusic()
    {
        await _sceneMusicsSOLoadedSource.Task.AttachExternalCancellation(_ct);
        return await ExtractSoundsFromAudioList(_sceneMusicsSO.AmbientMusic.StartGameAudio);
    }

    private async UniTask<List<AudioClip>> AsyncLoadEndGameMusic()
    {
        await _sceneMusicsSOLoadedSource.Task.AttachExternalCancellation(_ct);
        return await ExtractSoundsFromAudioList(_sceneMusicsSO.AmbientMusic.EndGameAudio);
    }

    private async UniTask<List<AudioClip>> ExtractSoundsFromAudioList(AudioList audioList)
    {
        if (audioList == null)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "AmbientMusic not found");
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

    private void StartPlayEndGameMusic()
    {
        AsyncPlayEndAmbientMusic().Forget();
    }

    private void Start()
    {
        AsyncPlayStartAmbientMusic().Forget();
    }

    private async UniTask AsyncPlayStartAmbientMusic()
    {
        await _ambientSoundsLoadedSource.Task.AttachExternalCancellation(_ct);

        var choicedAudioClip = GetRandomizedSound(_startGameMusics);

        SetMusicAndPlay(choicedAudioClip);
    }

    private async UniTask AsyncPlayEndAmbientMusic()
    {
        await _ambientSoundsLoadedSource.Task.AttachExternalCancellation(_ct);

        var choicedAudioClip = GetRandomizedSound(_endGameMusics);

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
        _source.Play();
    }
}
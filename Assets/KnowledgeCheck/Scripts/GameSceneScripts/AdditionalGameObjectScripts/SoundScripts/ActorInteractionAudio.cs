using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class ActorInteractionAudio : IDisposable
{
    private IAddressablesProvider _aP;
    private ActorInteractionAudioRepository _actorInteractionAudioRepository;
    private ActorInteractionAudioSO _actorInteractionAudioSO;

    private Dictionary<(SoundAction, SoundActor), List<AudioClip>> _audioData = new();

    private DisposableBag _dB;

    private UniTaskCompletionSource _actorInteractionAudioSOLoadedSource = new();
    private UniTaskCompletionSource _soundsLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAddressablesProvider aP,
        ActorInteractionAudioRepository actorInteractionAudioRepository
    )
    {
        _aP = aP;
        _actorInteractionAudioRepository = actorInteractionAudioRepository;

        AsyncLoadResource().Forget();
        AsyncUploadSounds().Forget();
    }

    public void Dispose()
    {
        _soundsLoadedSource.TrySetCanceled();
        _soundsLoadedSource = null;

        _actorInteractionAudioSOLoadedSource.TrySetCanceled();
        _actorInteractionAudioSOLoadedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();

        ClearDynamicProperties();
    }

    private void ClearDynamicProperties()
    {
        foreach (var audioClips in _audioData.Values)
        {
            audioClips.Clear();
        }
        _audioData.Clear();
    }

    private async UniTask AsyncLoadResource()
    {
        _actorInteractionAudioSO = await _actorInteractionAudioRepository.AsyncGetActorInteractionAudioSO(_ct.Token);

        _actorInteractionAudioSOLoadedSource.TrySetResult();
    }

    private async UniTaskVoid AsyncUploadSounds()
    {
        await LoadStepSounds();
        await LoadSwingSounds();
        await LoadGetDamageSounds();
        _soundsLoadedSource.TrySetResult();
    }

    private async UniTask LoadStepSounds()
    {
        SoundAction action = SoundAction.Steps;
        SoundActor stepType;
        stepType = SoundActor.Stone; _audioData.Add((action, stepType), await AsyncLoadSounds(action, stepType));
        stepType = SoundActor.Metal; _audioData.Add((action, stepType), await AsyncLoadSounds(action, stepType));
    }

    private async UniTask LoadSwingSounds()
    {
        SoundAction action = SoundAction.Swing;
        SoundActor swingType;
        swingType = SoundActor.Axe; _audioData.Add((action, swingType), await AsyncLoadSounds(action, swingType));
    }

    private async UniTask LoadGetDamageSounds()
    {
        SoundAction action = SoundAction.GetDamage;
        SoundActor materialDamageGetterType;
        materialDamageGetterType = SoundActor.Bone; /*  */ _audioData.Add((action, materialDamageGetterType), await AsyncLoadSounds(action, materialDamageGetterType));
        materialDamageGetterType = SoundActor.PlateArmour; _audioData.Add((action, materialDamageGetterType), await AsyncLoadSounds(action, materialDamageGetterType));
    }

    private async UniTask<List<AudioClip>> AsyncLoadSounds(SoundAction soundAction, SoundActor soundActor)
    {
        await _actorInteractionAudioSOLoadedSource.Task.AttachExternalCancellation(_ct.Token);

        var audioList = await _actorInteractionAudioSO.GetReferenceActorInteractionSounds(soundAction, soundActor, _ct.Token);
        if (audioList == null)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, $"({soundAction}, {soundActor}) sound not found");
            return null;
        }

        var referenceAudioClips = audioList.GetClipsReference();

        List<AudioClip> loadedAudioClips = new(referenceAudioClips.Count);

        foreach (var referenceAudioClip in referenceAudioClips)
        {
            loadedAudioClips.Add(await _aP.AsyncGetAddressablesDataFromReference<AudioClip>(referenceAudioClip, _ct.Token));
        }
        return loadedAudioClips;
    }

    public async UniTask<List<AudioClip>> AsyncGetSoundFromActionAndActor(SoundAction soundAction, SoundActor soundActor, CancellationToken ct)
    {
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, ct);
        await _soundsLoadedSource.Task.AttachExternalCancellation(linkedCts.Token);

        _audioData.TryGetValue((soundAction, soundActor), out var audioClips);
        return audioClips;
    }
}
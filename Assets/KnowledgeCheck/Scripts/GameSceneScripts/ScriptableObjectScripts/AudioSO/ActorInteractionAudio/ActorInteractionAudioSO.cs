using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorInteractionAudioSO", menuName = "Audio SO/Actor Interaction Audio SO")]
public class ActorInteractionAudioSO : ScriptableObject
{
    [SerializeField] private List<SoundActionClass> _materialSoundsList;
    private Dictionary<(SoundAction, SoundActor), AudioList> _materialSounds = new();

    private UniTaskCompletionSource _dictionaryGroupedSource = new();
    private CancellationTokenSource _ct = new();

    public void DisposeSO()
    {
        ClearTokens();
        ClearProperties();
    }

    public void SetDefaultState()
    {
        DisposeSO();
        SetNewProperties();
        SetNewTokens();
    }

    private void SetNewProperties()
    {
        _materialSounds = new();
    }

    private void SetNewTokens()
    {
        _dictionaryGroupedSource = new();
        _ct = new();
    }

    private void ClearProperties()
    {
        _materialSounds.Clear();
    }

    private void ClearTokens()
    {
        _dictionaryGroupedSource.TrySetCanceled();
        _dictionaryGroupedSource = null;

        _ct?.Cancel();
        _ct?.Dispose();
    }

    public async UniTask GroupAllSounds(CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        try
        {
            foreach (var soundActionClass in _materialSoundsList)
            {
                var (soundAction, soundActorsSounds) = soundActionClass.GetAllActorSounds();
                foreach (var soundActorClass in soundActorsSounds)
                {
                    var (soundActor, sounds) = soundActorClass.GetAllActorSounds();
                    _materialSounds.Add((soundAction, soundActor), sounds);

                    await UniTask.Yield(cancellationToken: linkedCTS.Token);
                }
                await UniTask.Yield(cancellationToken: linkedCTS.Token);
            }
        }
        catch (System.OperationCanceledException)
        {
            return;
        }

        _dictionaryGroupedSource.TrySetResult();
    }

    public async UniTask<AudioList> GetReferenceActorInteractionSounds(SoundAction soundAction, SoundActor soundActor, CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );

        await _dictionaryGroupedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        if (!_materialSounds.TryGetValue((soundAction, soundActor), out AudioList sounds))
        {
            ErrorMessageGenerator.GenerateSimpleError(this, $"({soundAction}, {soundActor}) sounds not found");
        }
        return sounds;
    }
}

[Serializable]
public class SoundActionClass
{
    [field: SerializeField] private SoundAction _soundAction;
    [field: SerializeField] private List<SoundActorClass> _soundActors = new();

    public (SoundAction soundAction, List<SoundActorClass> soundActorsSounds) GetAllActorSounds()
    {
        return (_soundAction, _soundActors);
    }
}

[Serializable]
public class SoundActorClass
{
    [field: SerializeField] private SoundActor _soundActor;
    [field: SerializeField] private AudioList _sounds = new();

    public (SoundActor soundActor, AudioList sounds) GetAllActorSounds()
    {
        return (_soundActor, _sounds);
    }
}

[Serializable]
public enum SoundAction
{
    Steps,
    GetDamage,
    Swing
}

[Serializable]
public enum SoundActor
{
    Nothing,
    Axe,
    Bone,
    PlateArmour,
    Stone,
    Metal
}
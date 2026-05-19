using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(IDamagable))]
public class CharacterEventSound : MonoBehaviour
{
    private AudioSource _source;
    private IDamagable _damagableObject;

    private ActorInteractionAudio _sounds;

    private List<AudioClip> _currentWeaponSwingSounds = new();
    private List<AudioClip> _currentGetDamageSounds = new();
    private List<AudioClip> _currentStepSounds = new();

    private Dictionary<SoundActor, List<AudioClip>> _weaponSwingSounds = new();
    private Dictionary<SoundActor, List<AudioClip>> _getDamageSounds = new();
    private Dictionary<SoundActor, List<AudioClip>> _stepSounds = new();


    [SerializeField] private CheckGroundUnderFoot _leftFoot;
    [SerializeField] private CheckGroundUnderFoot _rightFoot;

    private UniTaskCompletionSource _allSoundLoadedSource = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(ActorInteractionAudio sounds)
    {
        _sounds = sounds;

        _ct = gameObject.GetCancellationTokenOnDestroy();

        _source = GetComponent<AudioSource>();
        _damagableObject = GetComponent<IDamagable>();

        FillSounds().Forget();
        SetDefaultSounds().Forget();
    }

    private void OnDestroy()
    {
        _allSoundLoadedSource.TrySetCanceled();
        _allSoundLoadedSource = null;

        ClearDynamicProperties();
    }

    private void ClearDynamicProperties()
    {
        _currentWeaponSwingSounds.Clear();
        _currentGetDamageSounds.Clear();
        _currentStepSounds.Clear();

        foreach (var soundList in _weaponSwingSounds.Values)
        {
            soundList.Clear();
        }
        _weaponSwingSounds.Clear();

        foreach (var soundList in _getDamageSounds.Values)
        {
            soundList.Clear();
        }
        _getDamageSounds.Clear();

        foreach (var soundList in _stepSounds.Values)
        {
            soundList.Clear();
        }
        _stepSounds.Clear();
    }

    private async UniTaskVoid FillSounds()
    {
        List<UniTask> tasks = new()
        {
            AsyncFillWeaponSwingSound(),
            AsyncFillGetDamageSound(),
            AsyncFillStepsSound()
        };

        await UniTask.WhenAll(tasks).AttachExternalCancellation(_ct);

        _allSoundLoadedSource.TrySetResult();
    }

    private async UniTask AsyncFillWeaponSwingSound()
    {
        SoundAction soundAction = SoundAction.Swing;
        SoundActor soundActor = SoundActor.Axe;

        _weaponSwingSounds.Add(soundActor, await _sounds.AsyncGetSoundFromActionAndActor(soundAction, soundActor, _ct));
    }

    private async UniTask AsyncFillGetDamageSound()
    {
        SoundAction soundAction = SoundAction.GetDamage;
        if (_damagableObject as Player)
        {
            SoundActor soundActor = SoundActor.PlateArmour;
            _getDamageSounds.Add(soundActor, await _sounds.AsyncGetSoundFromActionAndActor(soundAction, soundActor, _ct));
        }
        if (_damagableObject as Enemy)
        {
            SoundActor soundActor = SoundActor.Bone;
            _getDamageSounds.Add(soundActor, await _sounds.AsyncGetSoundFromActionAndActor(soundAction, soundActor, _ct));
        }
    }

    private async UniTask AsyncFillStepsSound()
    {
        SoundAction soundAction = SoundAction.Steps;
        SoundActor soundActor;

        soundActor = SoundActor.Stone; _stepSounds.Add(soundActor, await _sounds.AsyncGetSoundFromActionAndActor(soundAction, soundActor, _ct));
        soundActor = SoundActor.Metal; _stepSounds.Add(soundActor, await _sounds.AsyncGetSoundFromActionAndActor(soundAction, soundActor, _ct));
    }

    private async UniTaskVoid SetDefaultSounds()
    {
        await _allSoundLoadedSource.Task.AttachExternalCancellation(_ct);

        SetCurrentWeaponSwingSounds(SoundActor.Axe);

        if (_damagableObject as Player)
        {
            SoundActor soundActor = SoundActor.PlateArmour; SetCurrentGetDamageSounds(soundActor);
        }
        if (_damagableObject as Enemy)
        {
            SoundActor soundActor = SoundActor.Bone; /*  */ SetCurrentGetDamageSounds(soundActor);
        }
    }

    public void SetCurrentWeaponSwingSounds(SoundActor soundReproducer)
    {
        if (!_weaponSwingSounds.TryGetValue(soundReproducer, out var audioClips))
        {
            ErrorMessageGenerator.GenerateErrorMessage(this, $"({soundReproducer}) sound not found in WeaponSwing sounds", out string generatedErrorMessage);
            Debug.LogException(new Exception(generatedErrorMessage));
            return;
        }
        _currentWeaponSwingSounds = audioClips;
    }

    public void SetCurrentGetDamageSounds(SoundActor soundReproducer)
    {
        if (!_getDamageSounds.TryGetValue(soundReproducer, out var audioClips))
        {
            ErrorMessageGenerator.GenerateErrorMessage(this, $"({soundReproducer}) sound not found in GetDamage sounds", out string generatedErrorMessage);
            Debug.LogException(new Exception(generatedErrorMessage));
            return;
        }
        _currentGetDamageSounds = audioClips;
    }

    public void SetCurrentStepsSounds(SoundActor soundReproducer)
    {
        if (!_stepSounds.TryGetValue(soundReproducer, out var audioClips))
        {
            return;
        }
        _currentStepSounds = audioClips;
    }

    // Play in ("Attack" layer -> "Hit" state; "Base" layer -> "Hit" state)
    public void SwingSound()
    {
        if (_currentWeaponSwingSounds == null || _currentWeaponSwingSounds.Count < 1)
            return;

        var choicedAudioClip = GetRandomizedSound(_currentWeaponSwingSounds, out var pitch);

        if (choicedAudioClip == null) return;

        _source.pitch = pitch;
        _source.PlayOneShot(choicedAudioClip);
    }

    // Play in ("Base" layer -> "Impact" state)
    public void GetDamageSound()
    {
        if (_currentGetDamageSounds == null || _currentGetDamageSounds.Count < 1)
            return;

        var choicedAudioClip = GetRandomizedSound(_currentGetDamageSounds, out var pitch);

        if (choicedAudioClip == null) return;

        _source.pitch = pitch;
        _source.PlayOneShot(choicedAudioClip);
    }

    // Play in ("Base" layer -> every motion in "IdleAndMotionBT" state)
    private void PlayStepSound()
    {
        if (_currentStepSounds == null || _currentStepSounds.Count < 1)
            return;

        var choicedAudioClip = GetRandomizedSound(_currentStepSounds, out var pitch);

        if (choicedAudioClip == null) return;

        _source.pitch = pitch;
        _source.PlayOneShot(choicedAudioClip);
    }

    private AudioClip GetRandomizedSound(List<AudioClip> audioClips, out float pitch)
    {
        pitch = UnityEngine.Random.Range(0.8f, 1.2f);

        if (audioClips.Count < 1) return null;

        int index = UnityEngine.Random.Range(0, audioClips.Count - 1);

        return audioClips[index];
    }

    public void LeftFootStepSound()
    {
        CheckGround(_leftFoot);
        PlayStepSound();
    }
    public void RightFootStepSound()
    {
        CheckGround(_rightFoot);
        PlayStepSound();
    }

    private void CheckGround(CheckGroundUnderFoot foot)
    {
        GroundType groundType = foot.CheckGround();

        switch (groundType)
        {
            case GroundType.Stone:
                SetCurrentStepsSounds(GroundTypeToSoundActorParser(groundType));
                break;
            case GroundType.Metal:
                SetCurrentStepsSounds(GroundTypeToSoundActorParser(groundType));
                break;
            default:
                SetCurrentStepsSounds(GroundTypeToSoundActorParser(groundType));
                break;
        }
    }

    private SoundActor GroundTypeToSoundActorParser(GroundType groundType) => groundType switch
    {
        GroundType.Stone => SoundActor.Stone,
        GroundType.Metal => SoundActor.Metal,
        _ => SoundActor.Nothing
    };
}
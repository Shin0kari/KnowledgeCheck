using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class CharacterEventSound : MonoBehaviour
{
    private AudioSource _source;
    private IDamagable _damagableObject;

    private MaterialSounds _sounds;

    private AudioClip _currentWeaponSwingSound;
    private AudioClip _currentGetDamageSound;
    private AudioClip _currentStepSound;

    [SerializeField] private CheckGroundUnderFoot _leftFoot;
    [SerializeField] private CheckGroundUnderFoot _rightFoot;

    [Inject]
    private void Construct(MaterialSounds sounds)
    {
        _sounds = sounds;

        _source = GetComponent<AudioSource>();
        _damagableObject = GetComponentInParent<IDamagable>();

        FillCurrentSoundsFromDefault();
    }

    private void FillCurrentSoundsFromDefault()
    {
        // Заполняем данных персонажа все звуки, кроме звуков шагов 
        FillCurrentWeaponSwingSoundFromDefault();
        FillCurrentGetDamageSoundFromDefault();
    }

    private void FillCurrentWeaponSwingSoundFromDefault()
    {
        _currentWeaponSwingSound = _sounds.AxeSwingSound;
    }

    private void FillCurrentGetDamageSoundFromDefault()
    {
        if (_damagableObject as Player)
            _currentGetDamageSound = _sounds.GetHitPlateArmourSound;
        if (_damagableObject as Enemy)
            _currentGetDamageSound = _sounds.GetHitBoneSound;
    }

    public void SwingSound()
    {
        _source.PlayOneShot(_currentWeaponSwingSound);
    }

    public void GetDamageSound()
    {
        if (_currentGetDamageSound == null)
            return;
        _source.PlayOneShot(_currentGetDamageSound);
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

    private void PlayStepSound()
    {
        if (_currentStepSound == null)
            return;

        _source.PlayOneShot(_currentStepSound);
    }

    private void CheckGround(CheckGroundUnderFoot foot)
    {
        GroundType groundType = foot.CheckGround();

        _currentStepSound = groundType switch
        {
            GroundType.Stone => _sounds.StoneStepSound,
            GroundType.Metal => _sounds.MetalStepSound,
            _ => null,
        };
    }
}
using UnityEngine;
using Zenject;

public class MaterialSounds
{
    private AudioClip _stoneStepSound;
    private AudioClip _metalStepSound;
    private AudioClip _axeSwingSound;
    private AudioClip _getHitBoneSound;
    private AudioClip _getHitPlateArmourSound;

    public AudioClip StoneStepSound { get { return _stoneStepSound; } }
    public AudioClip MetalStepSound { get { return _metalStepSound; } }
    public AudioClip AxeSwingSound { get { return _axeSwingSound; } }
    public AudioClip GetHitBoneSound { get { return _getHitBoneSound; } }
    public AudioClip GetHitPlateArmourSound { get { return _getHitPlateArmourSound; } }

    [Inject]
    private void Construct(
        AudioClip stoneStepSound,
        AudioClip metalStepSound,
        AudioClip axeSwingSound,
        AudioClip getHitBoneSound,
        AudioClip getHitPlateArmourSound
    )
    {
        _stoneStepSound = stoneStepSound;
        _metalStepSound = metalStepSound;
        _axeSwingSound = axeSwingSound;
        _getHitBoneSound = getHitBoneSound;
        _getHitPlateArmourSound = getHitPlateArmourSound;
    }
}
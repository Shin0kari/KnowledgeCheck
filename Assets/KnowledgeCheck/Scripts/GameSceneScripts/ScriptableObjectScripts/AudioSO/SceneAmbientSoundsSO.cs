using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneAmbientSoundsSO", menuName = "Audio SO/Scene Ambient Sounds SO")]
public class SceneAmbientSoundsSO : ScriptableObject
{
    [field: SerializeField] private FireAudio _fireSounds = new();
    public FireAudio FireSounds => _fireSounds;

    [field: SerializeField] private WaterAudio _waterSounds = new();
    public WaterAudio WaterSounds => _waterSounds;
}

[Serializable]
public class FireAudio
{
    [field: SerializeField] private AudioList _bigFireSounds = new();
    public AudioList BigFireSounds => _bigFireSounds;

    [field: SerializeField] private AudioList _smallFireSounds = new();
    public AudioList SmallFireSounds => _smallFireSounds;
}

[Serializable]
public class WaterAudio
{
    [field: SerializeField] private AudioList _waterFallSounds = new();
    public AudioList WaterFallSounds => _waterFallSounds;
}
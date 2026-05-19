using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMusicsSO", menuName = "Audio SO/Scene Musics SO")]
public class SceneMusicsSO : ScriptableObject
{
    [field: SerializeField] private AmbientMusic _ambientMusic = new();
    public AmbientMusic AmbientMusic => _ambientMusic;
}

[Serializable]
public class AmbientMusic
{
    [field: SerializeField] private AudioList _startGameAudio;
    public AudioList StartGameAudio => _startGameAudio;

    [field: SerializeField] private AudioList _endGameAudio;
    public AudioList EndGameAudio => _endGameAudio;
}
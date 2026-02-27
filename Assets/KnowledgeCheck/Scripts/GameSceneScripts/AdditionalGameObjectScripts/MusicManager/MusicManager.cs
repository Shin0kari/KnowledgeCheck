using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour, IDisposable
{
    [SerializeField] private AudioClip _startGameMusic;
    [SerializeField] private AudioClip _endGameMusic;

    private ArenaController _arenaController;

    private AudioSource _source;

    [Inject]
    private void Construct(ArenaController arenaController)
    {
        _arenaController = arenaController;
        _source = GetComponent<AudioSource>();

        _arenaController.StopSpawnEnemy += StartEndGameMusic;
    }

    public void Dispose()
    {
        if (_arenaController != null)
            _arenaController.StopSpawnEnemy -= StartEndGameMusic;
    }

    private void StartEndGameMusic()
    {
        SetMusicAndPlay(_endGameMusic);
    }

    private void Start()
    {
        SetMusicAndPlay(_startGameMusic);
    }

    private void SetMusicAndPlay(AudioClip music)
    {
        if (_source == null)
            return;
        _source.clip = music;
        _source.Play();
    }
}
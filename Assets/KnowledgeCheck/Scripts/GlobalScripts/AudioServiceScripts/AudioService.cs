using UnityEngine;
using Zenject;

public class AudioService : IAudioService
{
    private AudioSource _startMenuSceneAmbient;
    private AudioSource _gameSceneAmbient;
    private AudioSource _sceneAudioSource;
    private AudioClip _clickSound;
    private AudioClip _clickPanelSound;

    [Inject]
    private void Construct(
        AudioSource startMenuSceneAmbient,
        AudioSource gameSceneAmbient,
        AudioClip clickSound,
        AudioClip clickPanelSound
    )
    {
        _startMenuSceneAmbient = startMenuSceneAmbient;
        _gameSceneAmbient = gameSceneAmbient;
        _clickSound = clickSound;
        _clickPanelSound = clickPanelSound;
    }

    public void ChangeSceneAudioSource(AudioSource sceneAudioSource)
    {
        _sceneAudioSource = sceneAudioSource;
    }

    public void OnEnteringGameScene()
    {
        _startMenuSceneAmbient.Play();
    }

    public void OnEnteringStartMenuScene()
    {
        _gameSceneAmbient.Play();
    }

    public void OnUIClick()
    {
        if (_sceneAudioSource == null)
            return;
        _sceneAudioSource.PlayOneShot(_clickSound);
    }

    public void OnUIClickButtonPanel()
    {
        _sceneAudioSource.PlayOneShot(_clickPanelSound);
    }
}
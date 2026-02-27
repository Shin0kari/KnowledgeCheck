using UnityEngine;

public interface IAudioService
{
    public void OnUIClick();
    public void OnUIClickButtonPanel();

    public void OnEnteringStartMenuScene();
    public void OnEnteringGameScene();

    public void ChangeSceneAudioSource(AudioSource sceneAudioSource);
}
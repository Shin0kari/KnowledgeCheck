using UnityEngine;

public interface IAudioService
{
    public void OnUIClick();
    public void OnUIClickButtonPanel();

    public void ChangeSceneAudioSource(AudioSource sceneAudioSource);
}
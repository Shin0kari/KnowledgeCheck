using UnityEngine;
using Zenject;

public class StartMenuBoostrap
{
    [Inject]
    private void Construct(IAudioService audioService, AudioSource audioSource)
    {
        audioService.ChangeSceneAudioSource(audioSource);
    }
}
using UnityEngine;
using Zenject;

public class StartMenuBoostrap
{
    [Inject]
    private void Construct(
        IGetGameData gameData,
        IAudioService audioService,
        AudioSource audioSource)
    {
        gameData.UpdateGameData();

        audioService.ChangeSceneAudioSource(audioSource);
    }
}
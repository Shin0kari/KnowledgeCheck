using UnityEngine;
using Zenject;

public class GameBoostrap
{
    [Inject]
    private void Construct(
        IGetGameData gameData,
        PlayerFactory playerFactory,
        // StorageFactory storageFactory,
        IAudioService audioService,
        AudioSource sceneAudioSource
    )
    {
        gameData.UpdateGameData();
        playerFactory.Enable();
        // storageFactory.Enable();

        audioService.ChangeSceneAudioSource(sceneAudioSource);
    }
}

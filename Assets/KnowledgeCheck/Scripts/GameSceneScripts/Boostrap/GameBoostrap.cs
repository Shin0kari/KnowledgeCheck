using UnityEngine;
using Zenject;

public class GameBoostrap
{
    [Inject]
    private void Construct(
        IGetGameData gameData,
        PlayerFactory playerFactory,
        HealthBarFactory healthBarFactory,
        CarFactory carFactory,
        StorageFactory storageFactory,
        IAudioService audioService,
        AudioSource sceneAudioSource
    )
    {
        gameData.UpdateGameData();
        playerFactory.Enable();
        // healthBarFactory.Enable();
        // carFactory.Enable();
        storageFactory.Enable();

        audioService.ChangeSceneAudioSource(sceneAudioSource);
    }
}

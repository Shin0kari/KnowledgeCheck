using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static SceneUtils;

public class LoadSave
{
    private IGetGameData _gameData;
    private IValidatorGameData _validator;
    private GameStarter _starter;

    [Inject]
    private void Construct(IGetGameData gameData, IValidatorGameData validator, GameStarter starter)
    {
        _gameData = gameData;
        _validator = validator;
        _starter = starter;
    }

    // Так как при нажатии кнопки любого сохранения в LoadMenu сохранение становится current, 
    // то механизм запуска игры не отличается от ContinueGame 
    public void StartProcess()
    {
        Debug.Log("[LOAD_SAVE]: Start process scene download.");
        var currentSave = _gameData.GetCurrentGameData();
        if (!_validator.ValidateGameData(currentSave.Item2))
        {
            return;
        }

        var asyncStartGame = _starter.StartGame(currentSave, SceneNames.GameScene);
        UniTask.WhenAll(asyncStartGame);
    }
}

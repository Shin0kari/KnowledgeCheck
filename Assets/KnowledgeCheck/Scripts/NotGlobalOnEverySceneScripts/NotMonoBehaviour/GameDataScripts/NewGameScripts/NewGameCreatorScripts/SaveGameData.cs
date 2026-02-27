using Cysharp.Threading.Tasks;
using Zenject;
using static SceneUtils;

public class SaveGameData
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

    public void StartProcess()
    {
        var currentSave = _gameData.GetCurrentGameData();
        if (!_validator.ValidateGameData(currentSave.Item2))
        {
            return;
        }

        _starter.StartGame(currentSave, SceneNames.GameScene);
    }
}

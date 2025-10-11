using Cysharp.Threading.Tasks;
using Zenject;
using static SceneUtils;

public class NewSave
{
    private IGetGameData _gameData;
    private GameDataChanger _gameDataChanger;
    private GameStarter _starter;

    [Inject]
    private void Construct(IGetGameData gameData, GameDataChanger gameDataChanger, GameStarter starter)
    {
        _gameData = gameData;
        _gameDataChanger = gameDataChanger;
        _starter = starter;
    }

    public void StartProcess()
    {
        _gameDataChanger.CreateSave();
        var currentSave = _gameData.GetCurrentGameData();

        var asyncStartGame = _starter.StartGame(currentSave, SceneNames.GameScene);
        UniTask.WhenAll(asyncStartGame);
    }
}

using Zenject;

public class QuitGameButtonWithSave : QuitGameButton
{
    private GameDataChanger _gameDataChanger;
    private GameData _gameData;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger, GameData gameData)
    {
        _gameDataChanger = gameDataChanger;
        _gameData = gameData;
    }

    protected override void QuitGame()
    {
        _gameDataChanger.UpdateSave(_gameData.GetCurrentGameData().saveName);
        base.QuitGame();
    }
}
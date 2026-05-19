using Zenject;

public class QuitGameButtonWithSave : QuitGameButton
{
    private GameDataChanger _gameDataChanger;
    private IGetGameData _gameData;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger, IGetGameData gameData)
    {
        _gameDataChanger = gameDataChanger;
        _gameData = gameData;
    }

    protected override void QuitGame()
    {
        _gameDataChanger.UpdateSave(_gameData.GetCurrentGameData().uuid);
        base.QuitGame();
    }
}
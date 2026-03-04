using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static SceneUtils;

public class NewSave
{
    private IGetGameData _gameData;
    private GameDataChanger _gameDataChanger;
    private NewGame _newGameStarter;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        GameDataChanger gameDataChanger,
        NewGame newGameStarter)
    {
        _gameData = gameData;
        _gameDataChanger = gameDataChanger;
        _newGameStarter = newGameStarter;
    }

    public void StartProcess()
    {
        if (_gameData.GetCurrentGameData().uuid == null)
            _newGameStarter.StartProcess();
        else
            _gameDataChanger.CreateSaveWithCurrentData();
    }
}

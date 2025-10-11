using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static SceneUtils;

public class NewGame
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
        Debug.Log("[NEW_GAME]: Create save.");
        _gameDataChanger.CreateSave();
        var currentSave = _gameData.GetCurrentGameData();

        var asyncStartGame = _starter.StartGame(currentSave, SceneNames.GameScene);
        UniTask.WhenAll(asyncStartGame);
    }
}
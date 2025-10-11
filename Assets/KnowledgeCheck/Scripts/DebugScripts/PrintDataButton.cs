using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class PrintDataButton : MonoBehaviour
{
    private GameData _gameData;

    [Inject]
    private void Construct(GameData gameData)
    {
        _gameData = gameData;
    }

    public void PrintGameData()
    {
        var saves = _gameData.GetAllGameDatas();
        var currentSave = _gameData.GetCurrentGameData();

        Debug.Log("DEBUG [GAME_DATA]: All game data: " + JsonConvert.SerializeObject(saves));
        Debug.Log("DEBUG [GAME_DATA]: Current game data: " + JsonConvert.SerializeObject(currentSave));
    }
}
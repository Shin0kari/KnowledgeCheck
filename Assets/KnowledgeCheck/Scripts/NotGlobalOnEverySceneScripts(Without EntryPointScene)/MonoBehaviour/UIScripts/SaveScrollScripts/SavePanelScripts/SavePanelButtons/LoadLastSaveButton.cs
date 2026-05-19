using UnityEngine;
using Zenject;

public class LoadLastSaveButton : ChoicedSaveButton
{
    private GameDataChanger _gameDataChanger;
    private IGetGameData _gameData;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger, IGetGameData gameData)
    {
        _gameDataChanger = gameDataChanger;
        _gameData = gameData;
    }

    protected override void ActionOnClick()
    {
        _gameDataChanger.ChangeCurrentSave(_gameData.GetCurrentGameData().uuid);
    }
}
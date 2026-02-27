using System;
using UnityEngine;
using Zenject;

public class SaveDataSaverButton : ChoicedSaveButton
{
    private GameDataChanger _gameDataChanger;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger)
    {
        _gameDataChanger = gameDataChanger;
    }

    protected override void ActionOnClick()
    {
        _gameDataChanger.UpdateSave(GetComponentInParent<SavePanel>().GetSaveName());
    }
}
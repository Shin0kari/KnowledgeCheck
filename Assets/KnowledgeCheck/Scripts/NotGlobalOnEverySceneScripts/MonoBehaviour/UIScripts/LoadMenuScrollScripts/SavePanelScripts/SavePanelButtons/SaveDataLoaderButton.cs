using System;
using UnityEngine;
using Zenject;

public class SaveDataLoaderButton : ChoicedSaveButton
{
    private GameDataChanger _gameDataChanger;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger)
    {
        _gameDataChanger = gameDataChanger;
    }

    protected override void ActionOnClick()
    {
        _gameDataChanger.ChangeCurrentSave(GetComponentInParent<SavePanel>().GetSaveName());
    }
}
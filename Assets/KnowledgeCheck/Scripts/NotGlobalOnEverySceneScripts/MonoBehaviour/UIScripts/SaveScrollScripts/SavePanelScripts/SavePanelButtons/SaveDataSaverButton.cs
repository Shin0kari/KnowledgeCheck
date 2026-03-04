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
        // Сначала удаляется выбранное сохранение, а на его место создаётся другое и становится текущим
        _gameDataChanger.UpdateSave(GetComponentInParent<SavePanel>().GetSaveUuid());
    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class SaveDataSaverButton : ChoicedSaveButton
{
    private IGetGameData _gameData;
    private GameDataChanger _gameDataChanger;
    private UpdateSaveFromInventory _saveUpdater;

    private bool _isCurrentSaveAvailable;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        GameDataChanger gameDataChanger,
        UpdateSaveFromInventory saveUpdater)
    {
        _gameData = gameData;
        _gameDataChanger = gameDataChanger;
        _saveUpdater = saveUpdater;
    }

    protected override void ActionOnClick()
    {
        AsyncAction().Forget();
    }

    private async UniTask AsyncAction()
    {
        _isCurrentSaveAvailable = _gameData.GetCurrentGameData().uuid != null;
        // Сначала удаляется выбранное сохранение, а на его место создаётся другое и становится текущим
        _gameDataChanger.UpdateSave(GetComponentInParent<SavePanel>().GetSaveUuid());
        try
        {
            if (!_isCurrentSaveAvailable)
            {
                await _saveUpdater.FillGameDataFromInventory();
                _gameDataChanger.UpdateSave(_gameData.GetCurrentGameData().uuid);
            }
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }
}
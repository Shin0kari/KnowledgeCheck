using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class ScrollNewSaveButton : UIButton, IChangeButtonVisible
{
    private IGetGameData _gameData;
    private GameDataChanger _gameDataChanger;
    private UpdateSaveFromInventory _saveUpdater;

    private bool _isCurrentSaveAvailable;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        GameDataChanger gameDataChanger,
        UpdateSaveFromInventory saveUpdater
    )
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
        _gameDataChanger.CreateSaveWithCurrentData();
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

    // public void NewSave()
    // {
    //     IsUsed?.Invoke();
    // }

    public void HideButton()
    {
        _button.gameObject.SetActive(false);
    }

    public void RevealButton()
    {
        _button.gameObject.SetActive(true);
    }
}
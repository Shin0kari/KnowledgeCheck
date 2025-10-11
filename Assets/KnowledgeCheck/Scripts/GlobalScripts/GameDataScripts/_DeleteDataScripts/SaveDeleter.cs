using System;
using UnityEngine;
using Zenject;

public class SaveDeleter : ISaveDeleter
{
    private IDeleteData _dataDeleter;
    private IGetGameData _gameData;

    [Inject]
    private void Construct(IGetGameData gameData, IDeleteData dataDeleter)
    {
        _gameData = gameData;
        _dataDeleter = dataDeleter;
    }

    public void TryDeleteSave(string saveName)
    {
        if (!_gameData.GetAllGameDatas().ContainsKey(saveName))
        {
            return;
        }

        DeleteSave(saveName);
    }

    public void DeleteSave(string saveName)
    {
        _gameData.DeleteChoicedSave(saveName);

        if (_gameData.GetCurrentGameData().saveName == saveName)
        {
            _gameData.SetNullCurrentSave();
        }
        _dataDeleter.DeleteSave(saveName);
    }
}
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

    public void TryDeleteSave(string uuid)
    {
        if (!_gameData.GetAllGameDatas().ContainsKey(uuid))
        {
            return;
        }

        DeleteSave(uuid);
    }

    public void DeleteSave(string uuid)
    {
        if (!_gameData.GetAllGameDatas().TryGetValue(uuid, out SaveData saveData))
            return;
        _gameData.DeleteChoicedSave(uuid);
        _dataDeleter.DeleteSave(saveData.SaveName, saveData.Uuid);
    }

    public void DeleteNotCurrentSave(string uuid)
    {
        if (_gameData.GetCurrentGameData().uuid != uuid)
            TryDeleteSave(uuid);
    }
}
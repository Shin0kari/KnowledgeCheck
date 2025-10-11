using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SaveUpdater : ISaveUpdater
{
    private ISaveCreator _saveCreator;
    private ISaveDeleter _saveDeleter;
    private IGetGameData _gameData;

    [Inject]
    private void Construct(ISaveCreator saveCreator, ISaveDeleter saveDeleter, IGetGameData gameData)
    {
        _saveCreator = saveCreator;
        _saveDeleter = saveDeleter;
        _gameData = gameData;
    }

    public void TryChangeCurrentSave(string saveName)
    {
        var saves = _gameData.GetAllGameDatas();
        if (!saves.ContainsKey(saveName))
        {
            return;
        }

        var oldCurrentSave = _gameData.GetCurrentGameData();

        Debug.Log("[SAVE_UPDATER]: Change currentSave Data.");

        // Обновление прошлого currentSave
        if (!(oldCurrentSave.saveName == null))
        {
            _saveDeleter.DeleteSave(oldCurrentSave.saveName);
            oldCurrentSave.saveData.IsCurrentSave = false;
            _saveCreator.CreateSave(oldCurrentSave.saveName, oldCurrentSave.saveData);
        }

        // Обновление currentSave
        UpdateCurrentSave(saveName, saves[saveName]);
    }

    public void TryChangeSaveName(string saveName, string newSaveName)
    {
        Debug.Log("[SAVE_UPDATER]: oldSaveName: " + saveName + "; NewSaveName: " + newSaveName);
        var saves = _gameData.GetAllGameDatas();
        if (!saves.ContainsKey(saveName))
        {
            return;
        }

        // Обновление saveData
        Debug.Log("[SAVE_UPDATER]: Save was found.");
        SaveData saveData = saves[saveName];
        UpdateSaveName(newSaveName, ref saveData);
        _saveDeleter.DeleteSave(saveName);
        _saveCreator.CreateSave(newSaveName, saveData);

        if (saveData.IsCurrentSave)
            UpdateCurrentSave(newSaveName, saveData);
    }

    private void UpdateCurrentSave(string saveName, SaveData saveData)
    {
        _gameData.SetCurrentSave((saveName, saveData));
    }

    // public void TryUpdateCurrentSave()
    // {
    //     throw new NotImplementedException();
    // }

    // public void TryUpdateSave(string saveName, SaveData saveData)
    // {
    //     throw new NotImplementedException();
    // }

    private void UpdateSaveName(string newSaveName, ref SaveData saveData)
    {
        saveData.SaveName = newSaveName;
    }
}
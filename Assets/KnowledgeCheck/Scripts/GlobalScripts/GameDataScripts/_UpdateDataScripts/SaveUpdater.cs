using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SaveUpdater : ISaveUpdater
{
    private ISaveCreator _saveCreator;
    private ISaveDeleter _saveDeleter;
    private ISaveData _dataSaver;
    private ILoadData _loader;
    private IGetGameData _gameData;

    [Inject]
    private void Construct(
        ISaveCreator saveCreator,
        ISaveDeleter saveDeleter,
        ISaveData dataSaver,
        ILoadData loader,
        IGetGameData gameData)
    {
        _saveCreator = saveCreator;
        _saveDeleter = saveDeleter;
        _dataSaver = dataSaver;
        _loader = loader;
        _gameData = gameData;
    }

    // Обновление текущего сохранения без сохранения несохранённый данных
    // Пояснение: Если мы текущее_сохр_1 не сохранили, то при загрузке сохр_2
    // в файл с текущим_сохр_1 не будут сохранены новые изменённые данные из текущее_сохр_1
    public void TryChangeCurrentSave(string saveName)
    {
        var saves = _gameData.GetAllGameDatas();
        if (!saves.ContainsKey(saveName))
        {
            return;
        }

        var oldCurrentSaveName = _gameData.GetCurrentGameData().saveName;

        // Обновление прошлого currentSave
        if (oldCurrentSaveName != null)
        {
            if (oldCurrentSaveName == saveName)
            {
                (string saveName, SaveData saveData) loadedSave = _loader.LoadSpecificSave(oldCurrentSaveName);
                UpdateCurrentSaveGameData(loadedSave.saveName, loadedSave.saveData);
                return;
            }

            _gameData.GetAllGameDatas().TryGetValue(oldCurrentSaveName, out SaveData oldCurrentSaveData);
            (string saveName, SaveData saveData) oldCurrentSave = (oldCurrentSaveName, oldCurrentSaveData);

            _saveDeleter.DeleteSave(oldCurrentSave.saveName);
            oldCurrentSave.saveData.IsCurrentSave = false;
            _saveCreator.CreateSave(oldCurrentSave.saveName, oldCurrentSave.saveData);
        }

        // Обновление currentSave
        UpdateCurrentSaveGameDataAndFileData(saveName, saves[saveName]);
    }

    public void TryChangeSaveName(string saveName, string newSaveName)
    {
        var saves = _gameData.GetAllGameDatas();
        if (!saves.ContainsKey(saveName))
        {
            return;
        }

        SaveData saveData = saves[saveName];
        UpdateSaveName(newSaveName, ref saveData);
        _saveDeleter.DeleteSave(saveName);
        _saveCreator.CreateSave(newSaveName, saveData);

        if (saveData.IsCurrentSave)
            UpdateCurrentSaveGameData(newSaveName, saveData);
    }

    private void UpdateCurrentSaveGameDataAndFileData(string saveName, SaveData saveData)
    {
        _gameData.SetCurrentSave((saveName, saveData));
        _dataSaver.SaveData((saveName, saveData));
    }

    private void UpdateCurrentSaveGameData(string saveName, SaveData saveData)
    {
        _gameData.SetCurrentSave((saveName, saveData));
    }

    public void TryUpdateSave(string oldSaveName)
    {
        var saves = _gameData.GetAllGameDatas();
        if (!saves.TryGetValue(oldSaveName, out SaveData saveData))
        {
            return;
        }

        _saveDeleter.DeleteSave(oldSaveName);
        _saveCreator.CreateSave(oldSaveName, saveData);

        if (saveData.IsCurrentSave)
            UpdateCurrentSaveGameData(oldSaveName, saveData);
    }

    private void UpdateSaveName(string newSaveName, ref SaveData saveData)
    {
        saveData.SaveName = newSaveName;
    }
}
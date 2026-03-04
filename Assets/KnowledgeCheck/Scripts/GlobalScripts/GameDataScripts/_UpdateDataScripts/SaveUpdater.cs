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
    public void TryChangeCurrentSave(string uuid)
    {
        var saves = _gameData.GetAllGameDatas();
        if (!saves.ContainsKey(uuid))
        {
            return;
        }

        // Обновление прошлого currentSave
        if (_gameData.GetCurrentGameData().uuid != null)
        {
            var oldCurrentSaveUuid = _gameData.GetCurrentGameData().saveData.Uuid;
            var oldCurrentSaveData = _gameData.GetCurrentGameData().saveData;

            if (oldCurrentSaveUuid == uuid)
            {
                (string uuid, SaveData saveData) loadedSave = _loader.LoadSpecificSave(oldCurrentSaveData.SaveName, oldCurrentSaveUuid);
                UpdateCurrentSaveGameData(loadedSave.uuid, loadedSave.saveData);
                return;
            }

            (string uuid, SaveData saveData) oldCurrentSave = (oldCurrentSaveUuid, oldCurrentSaveData);

            // если файл с таким именем существует, он его перезапишет и удаление не нужно
            // _saveDeleter.DeleteSave(oldCurrentSave.uuid);
            oldCurrentSave.saveData.IsCurrentSave = false;
            _saveCreator.CreateSave(oldCurrentSave.uuid, oldCurrentSave.saveData);
        }

        // Обновление currentSave
        UpdateCurrentSaveGameDataAndFileData(uuid, saves[uuid]);
    }

    public void TryChangeSaveName(string uuid, string newSaveName)
    {
        var saves = _gameData.GetAllGameDatas();
        if (!saves.TryGetValue(uuid, out SaveData saveData))
        {
            return;
        }

        _saveDeleter.DeleteSave(uuid);
        UpdateSaveName(newSaveName, ref saveData);
        _saveCreator.CreateSave(saveData.Uuid, saveData);

        if (saveData.IsCurrentSave)
            UpdateCurrentSaveGameData(saveData.Uuid, saveData);
    }

    private void UpdateCurrentSaveGameDataAndFileData(string uuid, SaveData saveData)
    {
        _gameData.SetCurrentSave((uuid, saveData));
        _dataSaver.SaveData((uuid, saveData));
    }

    private void UpdateCurrentSaveGameData(string uuid, SaveData saveData)
    {
        _gameData.SetCurrentSave((uuid, saveData));
    }

    public void TryUpdateSave(string oldSaveUuid)
    {
        var saves = _gameData.GetAllGameDatas();
        if (!saves.TryGetValue(oldSaveUuid, out SaveData _))
        {
            return;
        }

        SaveData saveData = _gameData.GetCurrentGameData().saveData;
        if (saveData == null)
            return;

        _saveCreator.CreateSave(oldSaveUuid, saveData);

        UpdateCurrentSaveGameData(oldSaveUuid, saveData);
    }

    private void UpdateSaveName(string newSaveName, ref SaveData saveData)
    {
        saveData.SaveName = newSaveName;
    }
}
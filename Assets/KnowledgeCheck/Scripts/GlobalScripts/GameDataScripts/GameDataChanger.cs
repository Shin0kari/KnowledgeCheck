using System;
using UnityEngine;
using Zenject;

public class GameDataChanger
{
    private ISaveCreator _creator;
    private ISaveUpdater _updater;
    private ISaveDeleter _deleter;

    public event Action SaveCreated;
    public event Action SaveDeleted;
    public event Action CurrentSaveUpdated;
    public event Action ChoicedSaveUpdated;

    [Inject]
    private void Construct(
        ISaveCreator creator,
        ISaveUpdater updater,
        ISaveDeleter deleter)
    {
        _creator = creator;
        _updater = updater;
        _deleter = deleter;
    }

    public void CreateSave()
    {
        var createdSave = _creator.TryCreateSave();
        _updater.TryChangeCurrentSave(createdSave.saveData.Uuid);

        SaveCreated?.Invoke();
    }

    public void CreateSave(string uuid, SaveData saveData)
    {
        _creator.CreateSave(uuid, saveData);

        SaveCreated?.Invoke();
    }

    public void CreateSaveWithCurrentData()
    {
        var createdSave = _creator.TryCreateSaveWithCurrentData();
        _updater.TryChangeCurrentSave(createdSave.saveData.Uuid);

        SaveCreated?.Invoke();
    }

    public void UpdateSave(string oldSaveUuid)
    {
        _updater.TryUpdateSave(oldSaveUuid);

        ChoicedSaveUpdated?.Invoke();
    }

    public void ChangeSaveName(string uuid, string saveName, string newSaveName)
    {
        if (saveName == newSaveName)
            return;

        _updater.TryChangeSaveName(uuid, newSaveName);

        ChoicedSaveUpdated?.Invoke();
    }

    public void ChangeCurrentSave(string uuid)
    {
        _updater.TryChangeCurrentSave(uuid);

        CurrentSaveUpdated?.Invoke();
    }


    public void DeleteSave(string uuid)
    {
        _deleter.TryDeleteSave(uuid);

        SaveDeleted?.Invoke();
    }

    public void DeleteNotCurrentSave(string uuid)
    {
        _deleter.DeleteNotCurrentSave(uuid);
    }
}

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
        Debug.Log("[GAME_DATA_CHANGER]: Try create save.");
        var createdSave = _creator.TryCreateSave();
        _updater.TryChangeCurrentSave(createdSave.Item1);

        SaveCreated?.Invoke();
    }

    public void CreateSave(string saveName, SaveData saveData)
    {
        _creator.CreateSave(saveName, saveData);

        SaveCreated?.Invoke();
    }


    // public void UpdateCurrentSave()
    // {
    //     _updater.TryUpdateCurrentSave();

    //     CurrentSaveUpdated?.Invoke();
    // }

    // public void UpdateSave(string saveName, SaveData saveData)
    // {
    //     _updater.TryUpdateSave(saveName, saveData);

    //     ChoicedSaveUpdated?.Invoke();
    // }

    public void ChangeSaveName(string saveName, string newSaveName)
    {
        _updater.TryChangeSaveName(saveName, newSaveName);

        ChoicedSaveUpdated?.Invoke();
    }

    public void ChangeCurrentSave(string saveName)
    {
        _updater.TryChangeCurrentSave(saveName);

        CurrentSaveUpdated?.Invoke();
    }


    public void DeleteSave(string saveName)
    {
        _deleter.TryDeleteSave(saveName);

        SaveDeleted?.Invoke();
    }

    // public void DeleteSave(string saveName, SaveData saveData)
    // {
    //     _deleter.TryDeleteSave(saveName, saveData);
    // }
}

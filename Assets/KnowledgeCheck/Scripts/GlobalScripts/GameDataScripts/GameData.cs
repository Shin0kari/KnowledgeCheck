using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class GameData : IGetGameData
{
    private Dictionary<string, SaveData> _saves;
    private (string uuid, SaveData saveData) _currentSave;
    private ILoadData _loader;
    private IValidatorGameData _validator;

    public event Action CurrentSaveUpdated;

    [Inject]
    private void Construct(ILoadData loader, IValidatorGameData validator)
    {
        _loader = loader;
        _validator = validator;
    }

    public void UpdateGameData()
    {
        _saves = _loader.LoadAllSavesData();

        DefinitionCurrentSaveData();
        CurrentSaveUpdated?.Invoke();
    }

    private void DefinitionCurrentSaveData()
    {
        _currentSave = (null, null);
        foreach (var save in _saves)
        {
            if (save.Value.IsCurrentSave)
            {
                _currentSave = (save.Key, save.Value);
                break;
            }
        }
    }

    public void SetCurrentSave((string uuid, SaveData saveData) currentSave)
    {
        try
        {
            _validator.ValidateGameData(currentSave.saveData);
            currentSave.saveData.IsCurrentSave = true;
            _currentSave = currentSave;
            CurrentSaveUpdated?.Invoke();
        }
        catch (Exception ex)
        {

            Debug.LogError($"Ошибка получения текущего сохранения: {ex.Message}");
        }
    }

    public void SetNullCurrentSave()
    {
        _currentSave = (null, null);
        CurrentSaveUpdated?.Invoke();
    }

    public IReadOnlyDictionary<string, SaveData> GetAllGameDatas()
        => new ReadOnlyDictionary<string, SaveData>(_saves);

    public (string uuid, SaveData saveData) GetCurrentGameData()
    {
        return _currentSave;
    }

    public void DeleteChoicedSave(string uuid)
    {
        if (_currentSave.uuid == uuid)
            SetNullCurrentSave();
        _saves.Remove(uuid);
    }

    public void AddSaveToAllSaves((string uuid, SaveData saveData) currentSave)
    {
        if (_saves.ContainsKey(currentSave.uuid))
        {
            _saves.Remove(currentSave.uuid);
        }
        _saves.Add(currentSave.uuid, currentSave.saveData);
    }
}

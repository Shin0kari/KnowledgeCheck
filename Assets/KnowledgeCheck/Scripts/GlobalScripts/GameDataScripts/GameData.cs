using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class GameData : IGetGameData, IInitializable
{
    private Dictionary<string, SaveData> _saves;
    private (string, SaveData) _currentSave;
    private ILoadData _loader;
    private IValidatorGameData _validator;

    [Inject]
    private void Construct(ILoadData loader, IValidatorGameData validator)
    {
        _loader = loader;
        _validator = validator;
    }

    public void Initialize()
    {
        _saves = _loader.LoadData();

        DefinitionCurrentSaveData();
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

    public void SetCurrentSave((string saveName, SaveData saveData) currentSave)
    {
        try
        {
            _validator.ValidateGameData(currentSave.saveData);
            currentSave.saveData.IsCurrentSave = true;
            _currentSave = currentSave;
        }
        catch (Exception ex)
        {

            Debug.LogError($"Ошибка получения текущего сохранения: {ex.Message}");
        }
    }

    public void SetNullCurrentSave()
    {
        _currentSave = (null, null);
    }

    public IReadOnlyDictionary<string, SaveData> GetAllGameDatas()
        => new ReadOnlyDictionary<string, SaveData>(_saves);

    public (string, SaveData) GetCurrentGameData()
    {
        return _currentSave;
    }

    public void DeleteChoicedSave(string saveName)
    {
        _saves.Remove(saveName);
    }

    public void AddSaveToAllSaves((string saveName, SaveData saveData) currentSave)
    {
        _saves.Add(currentSave.saveName, currentSave.saveData);
    }
}

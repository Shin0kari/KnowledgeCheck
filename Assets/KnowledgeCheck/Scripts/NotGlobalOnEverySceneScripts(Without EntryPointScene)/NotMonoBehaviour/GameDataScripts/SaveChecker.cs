using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class SaveChecker : IInitializable, IDisposable
{
    private GameDataChanger _gameDataChanger;

    private IGetGameData _gameData;

    public event Action<IReadOnlyDictionary<string, SaveData>> CreateAllObjectSignal;
    public event Action<IReadOnlyDictionary<string, SaveData>> DeleteAllObjectsSignal;
    public event Action<IReadOnlyDictionary<string, SaveData>> UpdatedAllObjectSignal;
    public event Action<SaveData> CreateObjectSignal;
    public event Action<(string uuid, SaveData saveData)> UpdateObjectSignal;
    public event Action IsCountDataChanged;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        GameDataChanger gameDataChanger)
    {
        _gameData = gameData;
        _gameDataChanger = gameDataChanger;
    }

    public void Dispose()
    {
        if (_gameDataChanger != null)
        {
            _gameDataChanger.SaveCreated -= CreateObjectData;
            _gameDataChanger.SaveDeleted -= DeleteObjectData;
            _gameDataChanger.CurrentSaveUpdated -= UpdateObjectData;
            _gameDataChanger.ChoicedSaveUpdated -= UpdatedAllObject;
        }

        CreateAllObjectSignal = null;
        DeleteAllObjectsSignal = null;
        UpdatedAllObjectSignal = null;
        CreateObjectSignal = null;
        UpdateObjectSignal = null;
        IsCountDataChanged = null;
    }

    public void Initialize()
    {
        _gameDataChanger.SaveCreated += CreateObjectData;
        _gameDataChanger.SaveDeleted += DeleteObjectData;
        _gameDataChanger.CurrentSaveUpdated += UpdateObjectData;
        _gameDataChanger.ChoicedSaveUpdated += UpdatedAllObject;

        CreateAllObjectData();
    }

    public void CreateAllObjectData()
    {
        CreateAllObjectSignal?.Invoke(_gameData.GetAllGameDatas());
        IsCountDataChanged?.Invoke();
    }

    private void CreateObjectData()
    {
        CreateObjectSignal?.Invoke(_gameData.GetCurrentGameData().saveData);
        IsCountDataChanged?.Invoke();
    }

    private void DeleteObjectData()
    {
        DeleteAllObjectsSignal?.Invoke(_gameData.GetAllGameDatas());
        IsCountDataChanged?.Invoke();
    }

    private void UpdateObjectData()
    {
        UpdateObjectSignal?.Invoke(_gameData.GetCurrentGameData());
    }

    private void UpdatedAllObject()
    {
        UpdatedAllObjectSignal?.Invoke(_gameData.GetAllGameDatas());
    }
}
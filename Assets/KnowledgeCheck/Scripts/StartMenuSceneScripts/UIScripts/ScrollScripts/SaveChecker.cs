using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class SaveChecker : IInitializable, IDisposable
{
    private GameDataChanger _gameDataChanger;

    private IGetGameData _gameData;
    private IUpdatedObject _updatedObject;

    public event Action IsCountDataChanged;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        GameDataChanger gameDataChanger,
        IUpdatedObject updatedObject)
    {
        _gameData = gameData;
        _gameDataChanger = gameDataChanger;
        _updatedObject = updatedObject;
    }

    public void Initialize()
    {
        _gameDataChanger.SaveCreated += CreateObjectData;
        _gameDataChanger.SaveDeleted += DeleteObjectData;
        _gameDataChanger.CurrentSaveUpdated += UpdateObjectData;
        _gameDataChanger.ChoicedSaveUpdated += UpdatedAllObject;

        CreateAllObjectData();
    }

    public void Dispose()
    {
        _gameDataChanger.SaveCreated -= CreateObjectData;
        _gameDataChanger.SaveDeleted -= DeleteObjectData;
        _gameDataChanger.CurrentSaveUpdated -= UpdateObjectData;
        _gameDataChanger.ChoicedSaveUpdated -= UpdatedAllObject;
    }

    private void CreateAllObjectData()
    {
        _updatedObject.CreateAllObjectData(_gameData.GetAllGameDatas());
        IsCountDataChanged?.Invoke();
    }

    private void CreateObjectData()
    {
        var currentSave = _gameData.GetCurrentGameData();

        _updatedObject.CreateObjectWithData(currentSave.Item2);
        IsCountDataChanged?.Invoke();
    }

    private void DeleteObjectData()
    {
        _updatedObject.DeleteAllMissingObject(_gameData.GetAllGameDatas());
        IsCountDataChanged?.Invoke();
    }

    private void UpdateObjectData()
    {
        var currentSave = _gameData.GetCurrentGameData();

        _updatedObject.UpdateCurrentObject(currentSave);
    }

    private void UpdatedAllObject()
    {
        _updatedObject.UpdateAllObject(_gameData.GetAllGameDatas());
    }
}
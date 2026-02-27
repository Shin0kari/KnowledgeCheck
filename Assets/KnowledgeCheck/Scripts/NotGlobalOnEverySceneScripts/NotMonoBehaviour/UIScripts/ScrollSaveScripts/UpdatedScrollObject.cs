using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UpdatedScrollObject : IUpdatedObject, IDisposable
{
    private IUpdateScroll _updateMethod;
    private SaveChecker _saveChecker;

    [Inject]
    private void Construct(IUpdateScroll updateMethod, SaveChecker saveChecker)
    {
        _updateMethod = updateMethod;
        _saveChecker = saveChecker;

        _saveChecker.CreateAllObjectSignal += CreateAllObjectData;
        _saveChecker.CreateObjectSignal += CreateObjectWithData;
        _saveChecker.DeleteAllObjectsSignal += DeleteAllMissingObject;
        _saveChecker.UpdateObjectSignal += UpdateCurrentObject;
        _saveChecker.UpdatedAllObjectSignal += UpdateAllObject;
    }

    public void Dispose()
    {
        _saveChecker.CreateAllObjectSignal -= CreateAllObjectData;
        _saveChecker.CreateObjectSignal -= CreateObjectWithData;
        _saveChecker.DeleteAllObjectsSignal -= DeleteAllMissingObject;
        _saveChecker.UpdateObjectSignal -= UpdateCurrentObject;
        _saveChecker.UpdatedAllObjectSignal -= UpdateAllObject;
    }

    public void CreateAllObjectData(IReadOnlyDictionary<string, SaveData> saves)
    {
        _updateMethod.CreateAllSaves(saves);
    }

    public void CreateObjectWithData(SaveData saveData)
    {
        _updateMethod.AddSave(saveData);
    }

    public void DeleteAllMissingObject(IReadOnlyDictionary<string, SaveData> saves)
    {
        _updateMethod.DeleteMissingSaves(saves);
    }

    public void UpdateCurrentObject((string newSaveName, SaveData saveData) currentSave)
    {
        _updateMethod.UpdateCurrentSave(currentSave);
    }

    public void UpdateAllObject(IReadOnlyDictionary<string, SaveData> saves)
    {
        _updateMethod.UpdateAllSaves(saves);
    }

    // public void Initialize()
    // {
    //     Debug.Log("[1_UpdatedScrollObject]: Init");
    // }
}

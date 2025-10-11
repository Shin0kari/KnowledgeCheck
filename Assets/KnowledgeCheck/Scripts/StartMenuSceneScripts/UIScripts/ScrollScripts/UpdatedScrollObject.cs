using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UpdatedScrollObject : IUpdatedObject
{
    private IUpdateScroll _updateMethod;

    [Inject]
    private void Construct(IUpdateScroll updateMethod)
    {
        _updateMethod = updateMethod;
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
}

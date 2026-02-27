using System.Collections.Generic;
using System.Collections.ObjectModel;

public interface IUpdatedObject
{
    public void CreateAllObjectData(IReadOnlyDictionary<string, SaveData> saves);
    public void CreateObjectWithData(SaveData item2);
    public void DeleteAllMissingObject(IReadOnlyDictionary<string, SaveData> saves);
    public void UpdateCurrentObject((string newSaveName, SaveData saveData) currentSave);
    public void UpdateAllObject(IReadOnlyDictionary<string, SaveData> saves);
}
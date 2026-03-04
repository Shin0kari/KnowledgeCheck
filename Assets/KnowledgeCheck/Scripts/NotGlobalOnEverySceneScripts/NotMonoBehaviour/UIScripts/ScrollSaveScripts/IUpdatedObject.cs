using System.Collections.Generic;
using System.Collections.ObjectModel;

public interface IUpdatedObject
{
    public void CreateAllObjectData(IReadOnlyDictionary<string, SaveData> saves);
    public void CreateObjectWithData(SaveData saveData);
    public void DeleteAllMissingObject(IReadOnlyDictionary<string, SaveData> saves);
    public void UpdateCurrentObject((string uuid, SaveData saveData) currentSave);
    public void UpdateAllObject(IReadOnlyDictionary<string, SaveData> saves);
}
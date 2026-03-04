using System.Collections.Generic;
using UnityEngine;

public interface ILoadData
{
    public Dictionary<string, SaveData> LoadAllSavesData();
    public (string uuid, SaveData saveData) LoadSpecificSave(string saveName, string uuid);
}
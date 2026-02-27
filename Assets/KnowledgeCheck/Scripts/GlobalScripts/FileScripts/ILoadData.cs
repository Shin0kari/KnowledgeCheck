using System.Collections.Generic;
using UnityEngine;

public interface ILoadData
{
    public Dictionary<string, SaveData> LoadAllSavesData();
    public (string, SaveData) LoadSpecificSave(string fileName);
}
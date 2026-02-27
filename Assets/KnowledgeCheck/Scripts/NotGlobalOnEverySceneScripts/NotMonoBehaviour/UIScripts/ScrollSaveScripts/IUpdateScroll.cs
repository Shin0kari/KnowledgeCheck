using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUpdateScroll
{
    public void CreateAllSaves(IReadOnlyDictionary<string, SaveData> saves);

    public void AddSave(SaveData saveData);

    public void DeleteMissingSaves(IReadOnlyDictionary<string, SaveData> saves);

    public void UpdateCurrentSave((string newSaveName, SaveData saveData) currentSave);

    public void UpdateAllSaves(IReadOnlyDictionary<string, SaveData> saves);
}
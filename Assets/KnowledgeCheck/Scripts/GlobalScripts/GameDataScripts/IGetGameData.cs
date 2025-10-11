using System.Collections.Generic;
using UnityEngine;

public interface IGetGameData
{
    public (string saveName, SaveData saveData) GetCurrentGameData();
    public IReadOnlyDictionary<string, SaveData> GetAllGameDatas();
    public void SetCurrentSave((string saveName, SaveData saveData) currentSave);
    public void SetNullCurrentSave();
    public void DeleteChoicedSave(string saveName);
    public void AddSaveToAllSaves((string saveName, SaveData saveData) currentSave);
}
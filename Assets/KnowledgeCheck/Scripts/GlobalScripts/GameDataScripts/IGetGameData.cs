using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGetGameData
{
    public (string uuid, SaveData saveData) GetCurrentGameData();
    public IReadOnlyDictionary<string, SaveData> GetAllGameDatas();
    public void SetCurrentSave((string uuid, SaveData saveData) currentSave);
    public void SetNullCurrentSave();
    public void DeleteChoicedSave(string uuid);
    public void AddSaveToAllSaves((string uuid, SaveData saveData) currentSave);
    public void UpdateGameData();

    public event Action CurrentSaveUpdated;
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUpdateScroll
{
    public List<GameObject> CreateAllSaves(IReadOnlyDictionary<string, SaveData> saves);

    public GameObject AddSave(SaveData saveData);

    public void DeleteMissingSaves(IReadOnlyDictionary<string, SaveData> saves);

    public GameObject UpdateCurrentSave((string newSaveName, SaveData saveData) currentSave);

    public List<GameObject> UpdateAllSaves(IReadOnlyDictionary<string, SaveData> saves);
}
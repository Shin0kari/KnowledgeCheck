using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public interface IUpdateScroll
{
    public void CreateAllSaves(IReadOnlyDictionary<string, SaveData> saves);

    public UniTaskVoid AddSave(SaveData saveData, CancellationToken ct);
    public UniTaskVoid AddSave(SaveData saveData);

    public void DeleteMissingSaves(IReadOnlyDictionary<string, SaveData> saves);

    public void UpdateCurrentSave((string uuid, SaveData saveData) currentSave);

    public void UpdateAllSaves(IReadOnlyDictionary<string, SaveData> saves);
}
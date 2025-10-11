using UnityEngine;
public interface ISaveData
{
    public bool SaveData((string saveName, SaveData saveData) save);
}

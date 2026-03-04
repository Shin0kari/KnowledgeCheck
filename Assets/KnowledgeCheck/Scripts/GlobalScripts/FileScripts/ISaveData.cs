using UnityEngine;
public interface ISaveData
{
    public bool SaveData((string uuid, SaveData saveData) save);
}

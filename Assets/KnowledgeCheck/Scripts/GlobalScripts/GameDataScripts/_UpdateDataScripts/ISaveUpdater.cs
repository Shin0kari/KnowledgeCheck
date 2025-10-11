using System;

public interface ISaveUpdater
{
    // public void TryUpdateCurrentSave();
    // public void TryUpdateSave(string saveName, SaveData saveData);
    public void TryChangeSaveName(string saveName, string newSaveName);
    public void TryChangeCurrentSave(string saveName);
}

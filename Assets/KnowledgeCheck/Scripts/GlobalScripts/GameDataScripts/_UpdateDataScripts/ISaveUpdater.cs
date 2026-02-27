using System;

public interface ISaveUpdater
{
    // public void TryUpdateCurrentSave();
    public void TryUpdateSave(string oldSaveName);
    public void TryChangeSaveName(string saveName, string newSaveName);
    public void TryChangeCurrentSave(string saveName);
}

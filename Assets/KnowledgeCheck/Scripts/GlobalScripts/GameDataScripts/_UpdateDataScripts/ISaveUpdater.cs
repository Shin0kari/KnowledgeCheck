using System;

public interface ISaveUpdater
{
    public void TryUpdateSave(string oldSaveUuid);
    public void UpdateSave(string uuid);
    public void TryChangeSaveName(string uuid, string newSaveName);
    public void TryChangeCurrentSave(string uuid);
}

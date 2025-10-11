using System;

public interface ISaveDeleter
{
    public void TryDeleteSave(string saveName);
    public void DeleteSave(string saveName);
}

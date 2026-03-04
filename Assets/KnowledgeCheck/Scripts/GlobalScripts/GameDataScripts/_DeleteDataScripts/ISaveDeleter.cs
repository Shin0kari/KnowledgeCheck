using System;

public interface ISaveDeleter
{
    public void TryDeleteSave(string uuid);
    public void DeleteSave(string uuid);
    public void DeleteNotCurrentSave(string uuid);
}

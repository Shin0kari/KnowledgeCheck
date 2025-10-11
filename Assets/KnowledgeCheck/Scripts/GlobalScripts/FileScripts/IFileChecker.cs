using UnityEngine;

public interface IFileChecker
{
    public void TryCheckPathExists(string path);
    public bool CheckPresenceFiles(string path);
    public bool CheckPresenceSaveFile(string filename, string path);
}
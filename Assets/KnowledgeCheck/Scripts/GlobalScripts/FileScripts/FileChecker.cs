using System.IO;
using System.Linq;
using UnityEngine;

public class FileChecker : IFileChecker
{
    public bool CheckPresenceFiles(string path)
    {
        return Directory.EnumerateFiles(path, "*" + FileExtension.JsonExtensions).Any();
    }

    public bool CheckPresenceSaveFile(string filename, string path)
    {
        return Directory.EnumerateFiles(path, filename).Any();
    }

    public void TryCheckPathExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
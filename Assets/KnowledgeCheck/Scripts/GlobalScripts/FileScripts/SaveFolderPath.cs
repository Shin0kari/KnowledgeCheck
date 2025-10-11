using System;
using System.IO;
using UnityEngine;
using Zenject;

public class SaveFolderPath
{
    private IFileChecker _fileChecker;
    private string _savesPath;
    public string SavesPath
    {
        get
        {
            _savesPath ??= GetSavesFolder();

            return _savesPath;
        }
        private set
        {
            _savesPath = GetSavesFolder();
        }
    }

    [Inject]
    private void Construct(IFileChecker fileChecker)
    {
        _fileChecker = fileChecker;
    }

    public static string GetSavesPath()
    {
        var savesPath = Application.platform switch
        {
            var platform when platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor
                => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            var platform when platform == RuntimePlatform.LinuxPlayer || platform == RuntimePlatform.LinuxEditor
                => Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            var platform when platform == RuntimePlatform.WebGLPlayer
                => Application.persistentDataPath,
            _ => Application.persistentDataPath,
        };
        savesPath = Path.Combine(savesPath, "KnowledgeCheck", "Saves");

        return savesPath;
    }

    public string GetSavesFolder()
    {
        var savesPath = GetSavesPath();

        _fileChecker.TryCheckPathExists(savesPath);

        return savesPath;
    }
}
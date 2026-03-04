using System;
using System.IO;
using UnityEngine;
using Zenject;

public class FileDataDeleter : IDeleteData
{
    private SaveFolderPath _savePath;

    [Inject]
    private void Construct(SaveFolderPath savePath)
    {
        _savePath = savePath;
    }

    public bool DeleteSave(string saveName, string uuid)
    {
        try
        {
            string fileName = saveName + FileExtension.SeparationMark + uuid + FileExtension.JsonExtensions;
            string fullPath = Path.Combine(_savePath.SavesPath, fileName);

            File.Delete(fullPath);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при удалении: {ex.Message}");
            return false;
        }
    }
}
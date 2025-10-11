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

    public bool DeleteSave(string saveName)
    {
        try
        {
            string fileName = saveName + FileExtension.JsonExtensions;
            string fullPath = Path.Combine(_savePath.SavesPath, fileName);

            File.Delete(fullPath);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении: {ex.Message}");
            return false;
        }
    }
}
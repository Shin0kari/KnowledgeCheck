using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class FileDataSaver : ISaveData
{
    private SaveFolderPath _savePath;
    private IFileChecker _fileChecker;

    [Inject]
    private void Construct(SaveFolderPath savePath, IFileChecker fileChecker)
    {
        _savePath = savePath;
        _fileChecker = fileChecker;
    }

    /// <summary>
    /// Создаёт или изменяет файл сохранения.
    /// </summary>
    /// <param name="save"></param>
    /// <returns></returns>
    public bool SaveData((string saveName, SaveData saveData) save)
    {
        // Используется в SaveCreator, но если вызвать не там, то возможно
        // будут отсутствовать данные, необходимо сделать проверку 
        // или сделать зависимост от ISaveCreator чтобы поменять public на private
        try
        {
            string fileName = save.saveName + FileExtension.JsonExtensions;
            string fullPath = Path.Combine(_savePath.SavesPath, fileName);

            if (_fileChecker.CheckPresenceSaveFile(fileName, _savePath.SavesPath))
            {
                Debug.Log("Сохранение с таким именем уже существует.");
            }

            var jsonSave = JsonConvert.SerializeObject(save.saveData);

            File.WriteAllText(fullPath, jsonSave);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении: {ex.Message}");
            return false;
        }
    }
}
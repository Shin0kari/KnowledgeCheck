using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class FileDataLoader : ILoadData
{
    const string ALL_FILENAMES = "*";

    private SaveFolderPath _savePath;
    private IValidatorGameData _validator;

    [Inject]
    private void Construct(SaveFolderPath savePath, IValidatorGameData validator)
    {
        _savePath = savePath;
        _validator = validator;
    }

    public Dictionary<string, SaveData> LoadAllSavesData()
    {
        Dictionary<string, SaveData> saves = new(); // <uuid, saveData>
        var filePaths = Directory.EnumerateFiles(_savePath.SavesPath, ALL_FILENAMES + FileExtension.JsonExtensions);

        foreach (var filePath in filePaths)
        {
            try
            {
                SaveData data = GetDataFromFile(filePath);
                if (_validator.ValidateGameData(data))
                {
                    saves.Add(data.Uuid, data);
                }
                else
                {
                    Debug.Log($"[FILE_DATA_LOADER]: data from file `{filePath}` is not valid.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FILE_DATA_LOADER]: error - {ex.Message}");
            }
        }

        return saves;
    }

    public (string uuid, SaveData saveData) LoadSpecificSave(string saveName, string uuid)
    {
        (string uuid, SaveData saveData) save = new();
        var filePaths = Directory.EnumerateFiles(_savePath.SavesPath, saveName + FileExtension.SeparationMark + uuid + FileExtension.JsonExtensions);
        try
        {
            SaveData data = GetDataFromFile(filePaths.First());
            if (_validator.ValidateGameData(data))
            {
                save = (data.Uuid, data);
            }
            else
            {
                Debug.Log($"[FILE_DATA_LOADER]: data from file `{filePaths}` is not valid.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FILE_DATA_LOADER]: error - {ex.Message}");
        }
        return save;
    }

    private SaveData GetDataFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);

        JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Binder = new ItemSerializationBinder()
        };

        SaveData data = JsonConvert.DeserializeObject<SaveData>(json, settings);

        return data;
    }
}

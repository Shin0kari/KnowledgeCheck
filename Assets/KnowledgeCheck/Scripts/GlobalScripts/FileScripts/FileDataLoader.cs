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
        Dictionary<string, SaveData> saves = new();
        var fileNames = Directory.EnumerateFiles(_savePath.SavesPath, ALL_FILENAMES + FileExtension.JsonExtensions);

        foreach (var fileName in fileNames)
        {
            try
            {
                SaveData data = GetDataFromFile(fileName);
                if (_validator.ValidateGameData(data))
                {
                    saves.Add(data.SaveName, data);
                }
                else
                {
                    Debug.Log($"[FILE_DATA_LOADER]: data from file `{fileName}` is not valid.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FILE_DATA_LOADER]: error - {ex.Message}");
            }
        }

        return saves;
    }

    public (string, SaveData) LoadSpecificSave(string saveName)
    {
        (string saveName, SaveData saveData) save = new();
        var fileName = Directory.EnumerateFiles(_savePath.SavesPath, saveName + FileExtension.JsonExtensions);
        try
        {
            SaveData data = GetDataFromFile(fileName.First());
            if (_validator.ValidateGameData(data))
            {
                save = (data.SaveName, data);
            }
            else
            {
                Debug.Log($"[FILE_DATA_LOADER]: data from file `{fileName}` is not valid.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FILE_DATA_LOADER]: error - {ex.Message}");
        }
        return save;
    }

    private SaveData GetDataFromFile(string fileName)
    {
        string json = File.ReadAllText(fileName);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Binder = new ItemSerializationBinder()
        };

        SaveData data = JsonConvert.DeserializeObject<SaveData>(json, settings);

        return data;
    }
}

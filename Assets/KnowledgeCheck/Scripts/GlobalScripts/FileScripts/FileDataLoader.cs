using System;
using System.Collections.Generic;
using System.IO;
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

    public Dictionary<string, SaveData> LoadData()
    {
        Dictionary<string, SaveData> saves = new();
        var fileNames = Directory.EnumerateFiles(_savePath.SavesPath, ALL_FILENAMES + FileExtension.JsonExtensions);

        foreach (var fileName in fileNames)
        {
            // Debug.Log("[FILE_DATA_LOADER]: load data from file: " + fileName);
            // SaveData data = GetDataFromFile(fileName);
            // Debug.Log("[FILE_DATA_LOADER]: data is received.");
            // if (_validator.ValidateGameData(data))
            // {
            //     saves.Add(data.SaveName, data);
            //     Debug.Log("[FILE_DATA_LOADER]: data is added.");
            // }
            // else
            // {
            //     Debug.Log("[FILE_DATA_LOADER]: data is not valid.");
            // }

            try
            {
                Debug.Log("[FILE_DATA_LOADER]: load data from file: " + fileName);
                SaveData data = GetDataFromFile(fileName);
                Debug.Log("[FILE_DATA_LOADER]: data is received");
                if (_validator.ValidateGameData(data))
                {
                    saves.Add(data.SaveName, data);
                    Debug.Log("[FILE_DATA_LOADER]: data is added.");
                }
                else
                {
                    Debug.Log("[FILE_DATA_LOADER]: data is not valid.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка загрузки: {ex.Message}");
            }
        }

        return saves;
    }

    private SaveData GetDataFromFile(string fileName)
    {
        string json = File.ReadAllText(fileName);

        SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
        // SaveData data = JsonUtility.FromJson<SaveData>(json);

        return data;
    }
}

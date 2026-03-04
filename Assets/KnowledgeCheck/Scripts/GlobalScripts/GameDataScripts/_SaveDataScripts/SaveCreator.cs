using System;
using UnityEngine;
using Zenject;

public class SaveCreator : ISaveCreator
{
    private IStartDataFiller _startDataFiller;
    private ISaveData _dataSaver;
    private IGetGameData _gameData;

    [Inject]
    private void Construct(IStartDataFiller startDataFiller, ISaveData dataSaver, IGetGameData gameData)
    {
        _startDataFiller = startDataFiller;
        _dataSaver = dataSaver;
        _gameData = gameData;
    }

    /// <summary>
    /// Пытается создать сохранение, при нажатии кнопки NewGame или NewSave
    /// </summary>
    public (string uuid, SaveData saveData) TryCreateSave()
    {
        SaveData saveData = _startDataFiller.SetStartData();

        CreateSave(saveData.Uuid, saveData);

        return (saveData.Uuid, saveData);
    }

    public (string uuid, SaveData saveData) TryCreateSaveWithCurrentData()
    {
        if (_gameData.GetCurrentGameData().uuid != null)
        {
            var currentSaveData = _gameData.GetCurrentGameData().saveData;

            SaveData saveData = SaveDataRecordCloner.CloneSaveDataRecord(currentSaveData);

            saveData.Uuid = Guid.NewGuid().ToString();
            saveData.SaveName = _startDataFiller.GenerateSaveName();

            CreateSave(saveData.Uuid, saveData);

            return (saveData.Uuid, saveData);
        }
        else
        {
            return TryCreateSave();
        }
    }

    public bool CreateSave(string uuid, SaveData saveData)
    {
        if (_dataSaver.SaveData((uuid, saveData)))
        {
            _gameData.AddSaveToAllSaves((saveData.Uuid, saveData));

            return true;
        }
        return false;
    }
}

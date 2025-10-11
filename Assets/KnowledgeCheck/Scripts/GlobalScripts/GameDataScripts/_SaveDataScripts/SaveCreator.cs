using System;
using System.IO;
using TMPro;
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
    public (string, SaveData) TryCreateSave()
    {
        Debug.Log("[SAVE_CREATOR]: Try create save.");
        SaveData saveData = _startDataFiller.SetStartData();

        CreateSave(saveData.SaveName, saveData);

        return (saveData.SaveName, saveData);
    }

    public bool CreateSave(string saveName, SaveData saveData)
    {
        if (_dataSaver.SaveData((saveName, saveData)))
        {
            _gameData.AddSaveToAllSaves((saveName, saveData));

            return true;
        }
        return false;
    }
}

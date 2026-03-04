using System;
using TMPro;
using UnityEngine;

public interface ISaveCreator
{
    /// <summary>
    /// Пытается создать файл сохранения. 
    /// Срабатывает если нажата кнопка NewGame
    /// или при загрузке всех сохранений в Scroll 
    /// </summary>
    public (string uuid, SaveData saveData) TryCreateSave();
    public bool CreateSave(string uuid, SaveData saveData);
    public (string uuid, SaveData saveData) TryCreateSaveWithCurrentData();
}
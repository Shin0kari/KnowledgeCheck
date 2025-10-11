using System;
using TMPro;
using UnityEngine;

public interface ISaveCreator
{
    /// <summary>
    /// Пытается создать файл сохранения. 
    /// Срабатывает если нажата кнопка NewGame, NewSave
    /// или при загрузке всех сохранений в Scroll 
    /// </summary>
    public (string, SaveData) TryCreateSave();
    public bool CreateSave(string saveName, SaveData saveData);
}
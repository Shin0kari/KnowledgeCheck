using System;
using TMPro;
using UnityEngine;

public interface ISaveDataEditor
{
    public event Action EndEditSaveText;

    public void OnSelect();
    public void OnTextEndEdit();
    public string GetNewSaveName();
    public string GetOldSaveName();
}
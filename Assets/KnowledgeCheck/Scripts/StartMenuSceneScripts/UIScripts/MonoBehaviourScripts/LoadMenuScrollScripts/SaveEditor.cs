using System;
using TMPro;
using UnityEngine;

public class SaveEditor : SavePanel
{
    [SerializeField] private TMP_InputField _inputField;
    private string _oldSaveName;

    public event Action EndEditSaveText;

    public void OnSelect()
    {
        _inputField.text = _saveName.text;
        _oldSaveName = new string(_saveName.text);
    }

    public void OnTextEndEdit()
    {
        _saveName.text = _inputField.text;
        EndEditSaveText?.Invoke();
    }

    public string GetNewSaveName()
    {
        return GetSaveName();
    }

    public string GetOldSaveName()
    {
        return _oldSaveName;
    }
}

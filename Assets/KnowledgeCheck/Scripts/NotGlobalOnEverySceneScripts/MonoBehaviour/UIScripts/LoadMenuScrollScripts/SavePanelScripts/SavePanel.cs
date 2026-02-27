
using System;
using TMPro;
using UnityEngine;
using Zenject;

public class SavePanel : AbstractSavePanel, ISaveDataEditor, ISaveDataDeleter, ISaveDataSelector
{
    [SerializeField] private ChoicedSaveButton _choicedSaveButton;
    [SerializeField] private DeleteChoicedSaveButton _deleteChoicedSaveButton;

    [SerializeField] private TMP_InputField _inputField;
    private string _oldSaveName;

    public event Action EndEditSaveText;

    // public void OnDestroy()
    // {
    // }

    public UIButton GetChoiceButton() => _choicedSaveButton;
    public DeleteChoicedSaveButton GetDeleteSaveButton() => _deleteChoicedSaveButton;

    public string GetNewSaveName() => GetSaveName();
    public string GetOldSaveName() => _oldSaveName;

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

    public class Factory : PlaceholderFactory<UnityEngine.Object, SavePanel> { }
}
using TMPro;
using UnityEngine;

public class SavePanel : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _saveName;
    [SerializeField] private LoadChoicedSaveButton _loadChoicedSaveButton;
    [SerializeField] private DeleteChoicedSaveButton _deleteChoicedSaveButton;

    public string GetSaveName()
    {
        return _saveName.text;
    }

    public void SetSaveName(string newSaveName)
    {
        _saveName.text = newSaveName;
    }

    public IButton GetLoadChoiceButton() => _loadChoicedSaveButton;
    public DeleteChoicedSaveButton GetDeleteSaveButton() => _deleteChoicedSaveButton;
}
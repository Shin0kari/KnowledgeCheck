using System;
using UnityEngine;
using UnityEngine.UI;

public class DeleteChoicedSaveButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private SavePanel _savePanel;
    [SerializeField] private DeleteSavePanel _deleteSavePanel;

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            _deleteSavePanel.SetDeletedSavePanel(_savePanel);
        });
    }

    public void SetDeleteSavePanel(DeleteSavePanel deleteSavePanel)
    {
        _deleteSavePanel = deleteSavePanel;
    }

    // private void OnDestroy()
    // {
    // }
}
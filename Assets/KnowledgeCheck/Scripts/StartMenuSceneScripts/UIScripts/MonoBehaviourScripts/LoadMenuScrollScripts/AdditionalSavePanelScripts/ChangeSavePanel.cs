using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChangeSavePanel : MonoBehaviour
{
    // _backgroundButton нужна на случай, если игрок нажмёт на ЛКМ 
    // за панелью с изменением сохранения, 
    // чтобы при этом закрылась панель изменения сохранения 
    [SerializeField] private Button _backgroundButton;
    [SerializeField] private GameObject _changeSavePanel;
    [SerializeField] private SaveEditor _saveEditor;

    private GameDataChanger _gameDataChanger;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger)
    {
        _gameDataChanger = gameDataChanger;
    }

    private void Start()
    {
        _backgroundButton.onClick.AddListener(() =>
        {
            ClosePanel();
        });

        _saveEditor.EndEditSaveText += ClosePanel;
        _saveEditor.EndEditSaveText += SaveGameData;
    }

    private void ClosePanel()
    {
        _changeSavePanel.SetActive(false);
    }

    private void SaveGameData()
    {
        _gameDataChanger.ChangeSaveName(_saveEditor.GetOldSaveName(), _saveEditor.GetNewSaveName());
    }
}
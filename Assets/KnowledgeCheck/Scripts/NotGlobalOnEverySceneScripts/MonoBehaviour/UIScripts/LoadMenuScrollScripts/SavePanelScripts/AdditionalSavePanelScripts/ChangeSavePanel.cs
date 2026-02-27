using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChangeSavePanel : MonoBehaviour
{
    // _backgroundButton нужна на случай, если игрок нажмёт на ЛКМ 
    // за панелью с изменением сохранения, 
    // чтобы при этом закрылась панель изменения сохранения 
    // [SerializeField] private Button _backgroundButton;
    [SerializeField] private GameObject _changeSavePanel;
    [SerializeField] private SavePanel _savePanel;

    private GameDataChanger _gameDataChanger;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger)
    {
        _gameDataChanger = gameDataChanger;
    }

    private void Start()
    {

        _savePanel.EndEditSaveText += ClosePanel;
        _savePanel.EndEditSaveText += SaveGameData;
    }

    private void ClosePanel()
    {
        _changeSavePanel.SetActive(false);
    }

    private void SaveGameData()
    {
        Debug.Log("ChangeSaveName");
        _gameDataChanger.ChangeSaveName(_savePanel.GetOldSaveName(), _savePanel.GetNewSaveName());
    }

    private void OnDestroy()
    {
        _savePanel.EndEditSaveText -= ClosePanel;
        _savePanel.EndEditSaveText -= SaveGameData;
    }
}